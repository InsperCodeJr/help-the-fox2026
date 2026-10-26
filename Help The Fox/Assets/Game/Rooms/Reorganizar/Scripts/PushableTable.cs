using UnityEngine;

public class PushableTable : MonoBehaviour
{
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private TableGrid tableGrid;
    private RoomOrganizationPuzzle puzzle;
    public Vector2Int GridPosition => gridPosition;

    public void Initialize(TableGrid grid, Vector2Int initialPosition)
    {
        gridPosition = initialPosition;
        SetTableGrid(grid);
    }

    public void BindPuzzle(RoomOrganizationPuzzle owner) => puzzle = owner;

    // Movement requests always go through the room's validated rules, including player adjacency.
    public bool TryMove(Vector2Int direction) => puzzle != null && puzzle.TryPushTable(this, direction);

    internal void SetGridPosition(Vector2Int position)
    {
        gridPosition = position;
        SnapToGrid();
    }

    public void SetTableGrid(TableGrid grid)
    {
        if (tableGrid != null) tableGrid.UnregisterTable(this);
        tableGrid = grid;
        if (tableGrid != null && isActiveAndEnabled) tableGrid.RegisterTable(this);
        SnapToGrid();
    }

    public void SnapToGrid()
    {
        if (tableGrid == null) return;
        transform.position = tableGrid.CellToWorld(gridPosition);
        transform.rotation = Quaternion.identity;
    }

    private void OnEnable() { if (tableGrid != null) tableGrid.RegisterTable(this); }
    private void OnDisable() { if (tableGrid != null) tableGrid.UnregisterTable(this); }
}
