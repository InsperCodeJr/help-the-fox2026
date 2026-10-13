using UnityEngine;
using UnityEngine.InputSystem;

public class RoomResetController : MonoBehaviour
{
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    [SerializeField] private KeyCode resetKey = KeyCode.R;

    public void TriggerReset()
    {
        if (puzzle != null)
        {
            puzzle.ResetPuzzle();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            TriggerReset();
        }
    }
}
