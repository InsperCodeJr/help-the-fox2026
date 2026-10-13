using UnityEngine;

public class PushableTable : MonoBehaviour
{
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private TableGrid tableGrid;

    public Vector2Int GridPosition => gridPosition;

    public void Initialize(TableGrid grid, Vector2Int initialPosition)
    {
        tableGrid = grid;
        gridPosition = initialPosition;
        SnapToGrid();
    }

    public void SetGridPosition(Vector2Int newPosition)
    {
        gridPosition = newPosition;
        SnapToGrid();
    }

    public void SetTableGrid(TableGrid grid)
    {
        tableGrid = grid;
        SnapToGrid();
    }

    public bool TryMove(Vector2Int direction)
    {
        if (tableGrid == null)
        {
            return false;
        }

        Vector2Int targetCell = gridPosition + direction;

        if (tableGrid.IsCellBlocked(targetCell, this))
        {
            return false;
        }

        gridPosition = targetCell;
        SnapToGrid();
        return true;
    }

    public void SnapToGrid()
    {
        if (tableGrid == null)
        {
            return;
        }

        transform.position = tableGrid.CellToWorld(gridPosition);
    }

    private void OnEnable()
    {
        if (tableGrid != null)
        {
            tableGrid.RegisterTable(this);
        }
    }

    private void OnDisable()
    {
        if (tableGrid != null)
        {
            tableGrid.UnregisterTable(this);
        }
    }
}
