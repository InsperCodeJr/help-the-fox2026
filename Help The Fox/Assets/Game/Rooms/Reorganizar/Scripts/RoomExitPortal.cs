using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Integration hook only. The corridor owner connects the requested event to the shared door system.
public class RoomExitPortal : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    [SerializeField] private float interactionDistance = 0.75f;
    [SerializeField] private UnityEvent returnRequested = new UnityEvent();
    public event System.Action ReturnRequested;
    public UnityEvent OnReturnRequested => returnRequested;
    public bool PlayerIsClose => player != null &&
        Vector2.Distance(transform.position, player.position) <= interactionDistance;

    public void Initialize(Transform roomPlayer, RoomOrganizationPuzzle owner)
    {
        player = roomPlayer;
        puzzle = owner;
    }

    public void RequestReturn()
    {
        if (!PlayerIsClose || (puzzle != null && puzzle.InputBlocked)) return;
        returnRequested.Invoke();
        ReturnRequested?.Invoke();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) RequestReturn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
