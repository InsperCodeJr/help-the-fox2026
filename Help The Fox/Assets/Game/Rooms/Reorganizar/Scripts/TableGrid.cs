using System.Collections.Generic;
using UnityEngine;

public class TableGrid : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private Vector2 cellSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2Int origin = Vector2Int.zero;
    [SerializeField] private Vector2Int boardSize = new Vector2Int(12, 8);

    [Header("Walls")]
    [SerializeField] private List<Vector2Int> wallCells = new();

    private readonly List<PushableTable> registeredTables = new();

    public Vector2 CellSize => cellSize;
    public Vector2Int Origin => origin;
    public Vector2Int BoardSize => boardSize;

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3((origin.x + cell.x) * cellSize.x, (origin.y + cell.y) * cellSize.y, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - new Vector3(origin.x * cellSize.x, origin.y * cellSize.y, 0f);
        return new Vector2Int(Mathf.RoundToInt(localPosition.x / cellSize.x), Mathf.RoundToInt(localPosition.y / cellSize.y));
    }

    public bool IsCellInsideBoard(Vector2Int cell)
    {
        return cell.x >= origin.x && cell.x < origin.x + boardSize.x &&
               cell.y >= origin.y && cell.y < origin.y + boardSize.y;
    }

    public bool IsCellBlocked(Vector2Int cell, PushableTable ignoredTable = null)
    {
        if (!IsCellInsideBoard(cell))
        {
            return true;
        }

        if (wallCells.Contains(cell))
        {
            return true;
        }

        foreach (PushableTable table in registeredTables)
        {
            if (table == null || table == ignoredTable)
            {
                continue;
            }

            if (table.GridPosition == cell)
            {
                return true;
            }
        }

        return false;
    }

    public PushableTable GetTableAt(Vector2Int cell)
    {
        foreach (PushableTable table in registeredTables)
        {
            if (table != null && table.GridPosition == cell)
            {
                return table;
            }
        }

        return null;
    }

    public void RegisterTable(PushableTable table)
    {
        if (table == null || registeredTables.Contains(table))
        {
            return;
        }

        registeredTables.Add(table);
    }

    public void UnregisterTable(PushableTable table)
    {
        if (table == null)
        {
            return;
        }

        registeredTables.Remove(table);
    }

    public void ForceSnapTables(IEnumerable<PushableTable> tables)
    {
        foreach (PushableTable table in tables)
        {
            if (table != null)
            {
                table.SnapToGrid();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.7f, 1f, 0.25f);

        Vector3 bottomLeft = new Vector3(origin.x * cellSize.x, origin.y * cellSize.y, 0f);
        Vector3 size = new Vector3(boardSize.x * cellSize.x, boardSize.y * cellSize.y, 1f);
        Gizmos.DrawCube(bottomLeft + size * 0.5f, size);
    }
}
