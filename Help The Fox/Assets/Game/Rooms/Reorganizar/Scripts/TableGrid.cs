using System.Collections.Generic;
using UnityEngine;

public class TableGrid : MonoBehaviour
{
    [SerializeField] private Vector2 cellSize = Vector2.one;
    [SerializeField] private Vector2Int origin = Vector2Int.zero;
    [SerializeField] private Vector2Int boardSize = new Vector2Int(14, 10);
    [SerializeField] private List<Vector2Int> wallCells = new List<Vector2Int>();
    private readonly HashSet<PushableTable> registeredTables = new HashSet<PushableTable>();

    public Vector2 CellSize => cellSize;
    public Vector2Int BoardSize => boardSize;
    public IReadOnlyList<Vector2Int> WallCells => wallCells;
    public Vector2Int Origin => origin;

    public void Configure(Vector2Int size, IEnumerable<Vector2Int> walls)
    {
        boardSize = size;
        wallCells = new List<Vector2Int>(walls);
    }

    // Cell indices always start at zero. Origin offsets only their world-space positions.
    public Vector3 CellToWorld(Vector2Int cell) => transform.TransformPoint(
        new Vector3((origin.x + cell.x) * cellSize.x, (origin.y + cell.y) * cellSize.y, 0f));

    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = transform.InverseTransformPoint(world);
        return new Vector2Int(Mathf.RoundToInt(local.x / cellSize.x) - origin.x,
            Mathf.RoundToInt(local.y / cellSize.y) - origin.y);
    }

    public bool IsCellInsideBoard(Vector2Int cell) => cell.x >= 0 && cell.x < boardSize.x &&
        cell.y >= 0 && cell.y < boardSize.y;

    public bool IsCellBlocked(Vector2Int cell, PushableTable ignoredTable = null)
    {
        if (!IsCellInsideBoard(cell) || wallCells.Contains(cell)) return true;
        foreach (PushableTable table in registeredTables)
            if (table != null && table != ignoredTable && table.GridPosition == cell) return true;
        return false;
    }

    public PushableTable GetTableAt(Vector2Int cell)
    {
        foreach (PushableTable table in registeredTables)
            if (table != null && table.GridPosition == cell) return table;
        return null;
    }

    public void RegisterTable(PushableTable table) { if (table != null) registeredTables.Add(table); }
    public void UnregisterTable(PushableTable table) { if (table != null) registeredTables.Remove(table); }

    private void OnValidate()
    {
        cellSize.x = Mathf.Max(0.01f, cellSize.x);
        cellSize.y = Mathf.Max(0.01f, cellSize.y);
        boardSize.x = Mathf.Max(1, boardSize.x);
        boardSize.y = Mathf.Max(1, boardSize.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.7f, 1f, 0.3f);
        for (int x = 0; x < boardSize.x; x++)
            for (int y = 0; y < boardSize.y; y++)
                Gizmos.DrawWireCube(CellToWorld(new Vector2Int(x, y)),
                    new Vector3(cellSize.x, cellSize.y, 0.05f));
    }
}
