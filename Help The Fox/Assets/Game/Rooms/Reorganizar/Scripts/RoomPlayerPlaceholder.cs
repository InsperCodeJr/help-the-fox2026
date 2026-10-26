using UnityEngine;
using UnityEngine.InputSystem;

// Local test character. Replace this driver when the shared Player prefab becomes available.
public class RoomPlayerPlaceholder : MonoBehaviour
{
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    public void Initialize(RoomOrganizationPuzzle owner) => puzzle = owner;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (puzzle == null || keyboard == null || puzzle.InputBlocked) return;
        Vector2Int direction = Vector2Int.zero;
        // A single cardinal action per frame, even when diagonal keys are held.
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) direction = Vector2Int.up;
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) direction = Vector2Int.down;
        else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) direction = Vector2Int.left;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) direction = Vector2Int.right;
        if (direction != Vector2Int.zero) puzzle.TryMovePlayer(direction);
    }
}
