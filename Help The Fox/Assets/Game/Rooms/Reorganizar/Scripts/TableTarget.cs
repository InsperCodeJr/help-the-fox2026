using UnityEngine;

public class TableTarget : MonoBehaviour
{
    [SerializeField] private Vector2Int cellPosition;
    [SerializeField] private TableGrid tableGrid;
    [SerializeField] private bool showDuringPlay = true;
    public Vector2Int CellPosition => cellPosition;

    public void Initialize(TableGrid grid, Vector2Int cell)
    {
        tableGrid = grid;
        cellPosition = cell;
        transform.position = grid.CellToWorld(cell);
    }

    private void Start()
    {
        if (!showDuringPlay)
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>()) sprite.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 1f, 0.4f, 0.7f);
        Vector3 position = tableGrid != null ? tableGrid.CellToWorld(cellPosition) : transform.position;
        Gizmos.DrawWireCube(position, new Vector3(0.8f, 0.8f, 0.05f));
    }
}
