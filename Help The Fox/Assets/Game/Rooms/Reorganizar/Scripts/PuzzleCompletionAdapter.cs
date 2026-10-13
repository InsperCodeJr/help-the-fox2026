using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleCompletionAdapter : MonoBehaviour
{
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    [SerializeField] private LolaThoughtPanel thoughtPanel;
    [SerializeField] private GameObject letterEHighlight;
    [SerializeField] private string puzzleId = "Reorganizar";
    [SerializeField] private string thoughtText = "Agora consigo enxergar! As mesas formam a letra E. É melhor anotarmos.";
    [SerializeField] private bool showOnFirstCompletionOnly = true;

    private static readonly HashSet<string> locallyCompletedPuzzles = new();
    private bool completionRecorded;

    public static IReadOnlyCollection<string> LocalCompletedPuzzles => locallyCompletedPuzzles;

    public event System.Action<PuzzleNotification> PuzzleCompleted;

    private void OnEnable()
    {
        if (puzzle != null)
        {
            puzzle.Completed += HandlePuzzleCompleted;
        }
    }

    private void OnDisable()
    {
        if (puzzle != null)
        {
            puzzle.Completed -= HandlePuzzleCompleted;
        }
    }

    private void Start()
    {
        if (showOnFirstCompletionOnly && locallyCompletedPuzzles.Contains(puzzleId))
        {
            if (letterEHighlight != null)
            {
                letterEHighlight.SetActive(true);
            }

            if (thoughtPanel != null)
            {
                thoughtPanel.SetMessage(thoughtText);
            }
        }
    }

    private void HandlePuzzleCompleted()
    {
        if (completionRecorded)
        {
            return;
        }

        completionRecorded = true;

        if (locallyCompletedPuzzles.Contains(puzzleId))
        {
            return;
        }

        locallyCompletedPuzzles.Add(puzzleId);

        if (letterEHighlight != null)
        {
            letterEHighlight.SetActive(true);
        }

        if (thoughtPanel != null)
        {
            thoughtPanel.Show(thoughtText);
        }

        PuzzleNotification notification = new PuzzleNotification
        {
            PuzzleId = puzzleId,
            Message = thoughtText,
            DiscoveredLetters = new[] { "E" }
        };

        PuzzleCompleted?.Invoke(notification);
    }
}

[System.Serializable]
public class PuzzleNotification
{
    public string PuzzleId;
    public string Message;
    public string[] DiscoveredLetters;
}

public class LolaThoughtPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private string defaultMessage = "Agora consigo enxergar! As mesas formam a letra E. É melhor anotarmos.";

    public bool IsOpen => canvasGroup != null && canvasGroup.alpha > 0.01f;

    private void Awake()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(Close);
        }

        if (messageText != null)
        {
            messageText.text = defaultMessage;
        }

        CloseImmediate();
    }

    public void Show(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        if (gameObject != null)
        {
            gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void SetMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
    }

    public void Close()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private void CloseImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
