using UnityEngine;

public class RoomSpawnPoint : MonoBehaviour
{
    public bool TryPlacePlayer(Transform player, TableGrid grid)
    {
        if (player == null || grid == null) return false;
        Vector2Int cell = grid.WorldToCell(transform.position);
        if (grid.IsCellBlocked(cell)) return false;
        player.position = grid.CellToWorld(cell);
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
    }
}
