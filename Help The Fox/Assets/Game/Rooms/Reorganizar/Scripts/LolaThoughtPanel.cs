using UnityEngine;
using UnityEngine.InputSystem;

// Temporary room-only presentation. Replace its rendering with the team's thought UI later.
public class LolaThoughtPanel : MonoBehaviour
{
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    private string message;
    private GUIStyle bodyStyle;
    private GUIStyle letterStyle;
    public bool IsOpen { get; private set; }
    public void Initialize(RoomOrganizationPuzzle owner) => puzzle = owner;

    public void Show(string text)
    {
        message = text;
        IsOpen = true;
        if (puzzle != null) puzzle.SetInputBlocked(true);
    }

    public void Close()
    {
        IsOpen = false;
        if (puzzle != null) puzzle.SetInputBlocked(false);
    }

    private void OnDisable() => Close();

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (IsOpen && keyboard != null && (keyboard.enterKey.wasPressedThisFrame ||
            keyboard.numpadEnterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)) Close();
    }

    private void OnGUI()
    {
        if (!IsOpen) return;
        if (bodyStyle == null)
        {
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            letterStyle = new GUIStyle(bodyStyle) { fontSize = 66, fontStyle = FontStyle.Bold };
            letterStyle.normal.textColor = new Color(0.4f, 1f, 0.65f);
        }
        float width = Mathf.Min(560f, Screen.width - 24f);
        float height = Mathf.Min(310f, Screen.height - 24f);
        Rect panel = new Rect((Screen.width - width) / 2f, (Screen.height - height) / 2f, width, height);
        GUI.Box(panel, GUIContent.none);
        GUILayout.BeginArea(new Rect(panel.x + 20, panel.y + 10, width - 40, height - 20));
        GUILayout.Label("E", letterStyle, GUILayout.Height(height * 0.3f));
        GUILayout.Label(message, bodyStyle, GUILayout.ExpandHeight(true));
        if (GUILayout.Button("Confirmar (Enter / Espaço)", GUILayout.Height(40))) Close();
        GUILayout.EndArea();
    }
}
