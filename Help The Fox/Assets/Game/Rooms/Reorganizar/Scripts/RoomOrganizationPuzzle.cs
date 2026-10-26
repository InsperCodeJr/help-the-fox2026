using System;
using System.Collections.Generic;
using UnityEngine;
using Cell = TableBoardState.Cell;

public class RoomOrganizationPuzzle : MonoBehaviour
{
    [SerializeField] private TableGrid tableGrid;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<PushableTable> tables = new List<PushableTable>();
    [SerializeField] private List<TableTarget> targets = new List<TableTarget>();
    [SerializeField] private GameObject letterEVisual = null;
    [SerializeField, Min(0.05f)] private float pushCooldownSeconds = 0.14f;
    private TableBoardState state;
    private float nextAllowedMoveTime;

    public bool IsSolved => state != null && state.IsSolved;
    public bool InputBlocked { get; private set; }
    public bool IsReady => state != null;
    public int OccupiedTargetCount => state != null ? state.OccupiedTargetCount : 0;
    public int RequiredTargetCount => targets.Count;
    public event Action Completed;
    public event Action PuzzleReset;

    public void Initialize(TableGrid grid, Transform player, IEnumerable<PushableTable> roomTables,
        IEnumerable<TableTarget> roomTargets)
    {
        tableGrid = grid;
        playerTransform = player;
        tables = new List<PushableTable>(roomTables);
        targets = new List<TableTarget>(roomTargets);
        BuildState();
    }

    private void Start()
    {
        if (state != null) return;
        if (tableGrid == null) tableGrid = GetComponent<TableGrid>();
        if (tables.Count == 0) tables.AddRange(GetComponentsInChildren<PushableTable>(true));
        if (targets.Count == 0) targets.AddRange(GetComponentsInChildren<TableTarget>(true));
        BuildState();
    }

    private void BuildState()
    {
        if (tableGrid == null || playerTransform == null)
        {
            Debug.LogError("Reorganizar: atribua a grade e o jogador local no Inspector.", this);
            return;
        }
        try
        {
            var walls = new List<Cell>();
            var tableCells = new List<Cell>();
            var targetCells = new List<Cell>();
            foreach (Vector2Int wall in tableGrid.WallCells) walls.Add(ToCell(wall));
            foreach (PushableTable table in tables)
            {
                if (table == null) throw new ArgumentException("Referência de mesa ausente.");
                table.SetTableGrid(tableGrid);
                table.BindPuzzle(this);
                tableCells.Add(ToCell(table.GridPosition));
            }
            foreach (TableTarget target in targets)
            {
                if (target == null) throw new ArgumentException("Referência de alvo ausente.");
                targetCells.Add(ToCell(target.CellPosition));
            }
            state = new TableBoardState(tableGrid.BoardSize.x, tableGrid.BoardSize.y, walls,
                tableCells, targetCells, ToCell(tableGrid.WorldToCell(playerTransform.position)));
            SynchronizeVisuals();
        }
        catch (ArgumentException error)
        {
            state = null;
            Debug.LogError("Reorganizar: configuração inválida: " + error.Message, this);
        }
    }

    public void SetInputBlocked(bool blocked) => InputBlocked = blocked;

    public bool TryMovePlayer(Vector2Int direction)
    {
        if (state == null || InputBlocked || Time.timeScale <= 0f || Time.time < nextAllowedMoveTime)
            return false;
        if (!ToCell(direction).IsCardinal) return false;
        nextAllowedMoveTime = Time.time + pushCooldownSeconds;
        bool wasSolved = state.IsSolved;
        if (!state.TryStep(ToCell(direction), out _)) return false;
        SynchronizeVisuals();
        if (!wasSolved && state.IsSolved) Completed?.Invoke();
        return true;
    }

    public bool TryPushTable(PushableTable table, Vector2Int direction)
    {
        if (state == null || table == null || !tables.Contains(table) || !ToCell(direction).IsCardinal)
            return false;
        if (state.Player + ToCell(direction) != ToCell(table.GridPosition)) return false;
        return TryMovePlayer(direction);
    }

    public void ResetPuzzle()
    {
        if (state == null || InputBlocked || !state.Reset()) return;
        nextAllowedMoveTime = 0f;
        SynchronizeVisuals();
        PuzzleReset?.Invoke();
    }

    public void RestoreSolvedLayout()
    {
        if (state == null) return;
        state.RestoreSolvedLayout();
        SynchronizeVisuals();
    }

    private void SynchronizeVisuals()
    {
        for (int i = 0; i < tables.Count; i++) tables[i].SetGridPosition(ToVector(state.TableAtIndex(i)));
        playerTransform.position = tableGrid.CellToWorld(ToVector(state.Player));
        if (letterEVisual != null) letterEVisual.SetActive(state.IsSolved);
    }

    private static Cell ToCell(Vector2Int vector) => new Cell(vector.x, vector.y);
    private static Vector2Int ToVector(Cell cell) => new Vector2Int(cell.X, cell.Y);
}
