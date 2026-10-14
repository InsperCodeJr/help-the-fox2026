using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomOrganizationPuzzle : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] private TableGrid tableGrid;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<PushableTable> tables = new();
    [SerializeField] private List<TableTarget> targets = new();
    [SerializeField] private GameObject letterEVisual;

    [Header("Movement")]
    [SerializeField] private float pushCooldownSeconds = 0.12f;
    [SerializeField] private float playerReachDistance = 1.5f;

    [Header("Reset")]
    [SerializeField] private bool lockResetAfterCompletion = true;

    private readonly Dictionary<PushableTable, Vector2Int> initialTablePositions = new();
    private float nextAllowedPushTime;
    private bool isSolved;

    public bool IsSolved => isSolved;
    public int OccupiedTargetCount => CountOccupiedTargets();
    public int RequiredTargetCount => targets.Count;
    public event System.Action Completed;
    public event System.Action Reset;

    // private void Reset()
    // {
    //     ConfigureReferences();
    //     SaveInitialTableState();
    // }

    private void Awake()
    {
        ConfigureReferences();
        SaveInitialTableState();
        ApplyBoardState();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!lockResetAfterCompletion || !isSolved)
            {
                ResetPuzzle();
            }
        }

        if (Time.time < nextAllowedPushTime)
        {
            return;
        }

        Vector2Int direction = ReadMovementInput();
        if (direction == Vector2Int.zero)
        {
            return;
        }

        if (playerTransform == null)
        {
            playerTransform = ResolvePlayer();
        }

        TryPushTableInFrontOfPlayer(direction);
    }

    public void ResetPuzzle()
    {
        if (lockResetAfterCompletion && isSolved)
        {
            return;
        }

        foreach (PushableTable table in tables)
        {
            if (table == null)
            {
                continue;
            }

            if (initialTablePositions.TryGetValue(table, out Vector2Int startingCell))
            {
                table.SetGridPosition(startingCell);
            }
        }

        if (letterEVisual != null)
        {
            letterEVisual.SetActive(false);
        }

        isSolved = false;
        nextAllowedPushTime = 0f;
        Reset?.Invoke();
    }

    public bool TryPushTable(PushableTable table, Vector2Int direction)
    {
        if (table == null || direction == Vector2Int.zero || Time.time < nextAllowedPushTime)
        {
            return false;
        }

        if (tableGrid == null)
        {
            return false;
        }

        Vector2Int targetCell = table.GridPosition + direction;

        if (tableGrid.IsCellBlocked(targetCell, table))
        {
            return false;
        }

        table.SetGridPosition(targetCell);
        nextAllowedPushTime = Time.time + pushCooldownSeconds;

        CheckForCompletion();
        return true;
    }

    public bool HasTargetOverlap()
    {
        HashSet<Vector2Int> occupiedTargetCells = new();

        foreach (TableTarget target in targets)
        {
            if (target == null)
            {
                continue;
            }

            if (tableGrid == null || !tableGrid.IsCellInsideBoard(target.CellPosition))
            {
                continue;
            }

            if (!occupiedTargetCells.Add(target.CellPosition))
            {
                return true;
            }
        }

        foreach (PushableTable table in tables)
        {
            if (table == null)
            {
                continue;
            }

            if (IsTargetCellOccupiedByTable(table.GridPosition))
            {
                if (!occupiedTargetCells.Add(table.GridPosition))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int CountOccupiedTargets()
    {
        int totals = 0;

        foreach (TableTarget target in targets)
        {
            if (target == null)
            {
                continue;
            }

            if (IsTargetCellOccupiedByTable(target.CellPosition))
            {
                totals++;
            }
        }

        return totals;
    }

    public bool IsTargetCellOccupiedByTable(Vector2Int cell)
    {
        foreach (PushableTable table in tables)
        {
            if (table != null && table.GridPosition == cell)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2Int ReadMovementInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector2Int.zero;
        }

        Vector2Int direction = Vector2Int.zero;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            direction.x -= 1;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            direction.x += 1;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            direction.y -= 1;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            direction.y += 1;
        }

        if (direction.x != 0 && direction.y != 0)
        {
            direction = new Vector2Int(direction.x, 0);
        }

        return direction;
    }

    public void SetLetterEVisual(bool enabled)
    {
        if (letterEVisual != null)
        {
            letterEVisual.SetActive(enabled);
        }
    }

    private void ConfigureReferences()
    {
        if (tableGrid == null)
        {
            tableGrid = GetComponent<TableGrid>();
        }

        if (tables.Count == 0)
        {
            tables.AddRange(GetComponentsInChildren<PushableTable>(true));
        }

        if (targets.Count == 0)
        {
            targets.AddRange(GetComponentsInChildren<TableTarget>(true));
        }

        if (playerTransform == null)
        {
            playerTransform = ResolvePlayer();
        }
    }

    private void SaveInitialTableState()
    {
        initialTablePositions.Clear();

        foreach (PushableTable table in tables)
        {
            if (table == null)
            {
                continue;
            }

            if (!initialTablePositions.ContainsKey(table))
            {
                initialTablePositions.Add(table, table.GridPosition);
            }
        }
    }

    private void ApplyBoardState()
    {
        if (tableGrid == null)
        {
            return;
        }

        foreach (PushableTable table in tables)
        {
            if (table != null)
            {
                table.SetTableGrid(tableGrid);
                table.SnapToGrid();
            }
        }
    }

    private void TryPushTableInFrontOfPlayer(Vector2Int direction)
    {
        if (playerTransform == null || tableGrid == null)
        {
            return;
        }

        Vector2Int playerCell = tableGrid.WorldToCell(playerTransform.position);
        Vector2Int tableCellAhead = playerCell + direction;
        PushableTable table = tableGrid.GetTableAt(tableCellAhead);

        if (table == null)
        {
            return;
        }

        float distance = Vector2.Distance(playerTransform.position, table.transform.position);
        if (distance > playerReachDistance)
        {
            return;
        }

        TryPushTable(table, direction);
    }

    private Transform ResolvePlayer()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");

        if (foundPlayer == null)
        {
            foundPlayer = GameObject.Find("Player");
        }

        if (foundPlayer == null)
        {
            foundPlayer = GameObject.Find("player");
        }

        if (foundPlayer == null)
        {
            return null;
        }

        return foundPlayer.transform;
    }

    private void CheckForCompletion()
    {
        if (isSolved)
        {
            return;
        }

        if (targets.Count == 0 || HasTargetOverlap())
        {
            return;
        }

        if (CountOccupiedTargets() >= targets.Count)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        isSolved = true;

        if (letterEVisual != null)
        {
            letterEVisual.SetActive(true);
        }

        Completed?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (targets.Count == 0)
        {
            return;
        }

        Gizmos.color = new Color(0.3f, 1f, 0.7f, 0.6f);

        foreach (TableTarget target in targets)
        {
            if (target == null)
            {
                continue;
            }

            Vector3 worldPosition = target.transform.position;
            Gizmos.DrawWireCube(worldPosition, new Vector3(0.7f, 0.7f, 0.1f));
        }
    }
}
