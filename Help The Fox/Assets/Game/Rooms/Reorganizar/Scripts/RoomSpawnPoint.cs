using UnityEngine;

public class RoomSpawnPoint : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = new Color(0.2f, 0.8f, 1f, 0.9f);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
