using UnityEngine;
using UnityEngine.InputSystem;

public class RoomResetController : MonoBehaviour
{
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    public void Initialize(RoomOrganizationPuzzle owner) => puzzle = owner;
    public void TriggerReset() { if (puzzle != null) puzzle.ResetPuzzle(); }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) TriggerReset();
    }
}
