using UnityEngine;

public class TableTarget : MonoBehaviour
{
    [SerializeField] private Vector2Int cellPosition;
    [SerializeField] private Color gizmoColor = new Color(0.3f, 1f, 0.4f, 0.7f);
    [SerializeField] private float drawSize = 0.6f;

    public Vector2Int CellPosition => cellPosition;

    public void SetCellPosition(Vector2Int newPosition)
    {
        cellPosition = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, new Vector3(drawSize, drawSize, 0.05f));
    }
}
