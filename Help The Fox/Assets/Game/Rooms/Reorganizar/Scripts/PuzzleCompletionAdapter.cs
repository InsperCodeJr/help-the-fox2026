using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PuzzleCompletionAdapter : MonoBehaviour
{
    public const string CompletionMessage = "Agora consigo enxergar! As mesas formam a letra E. É melhor anotarmos.";
    [SerializeField] private RoomOrganizationPuzzle puzzle;
    [SerializeField] private LolaThoughtPanel thoughtPanel;
    [SerializeField] private string puzzleId = "Reorganizar";
    [SerializeField] private UnityEvent<string> puzzleIdCompleted = new UnityEvent<string>();
    private static readonly HashSet<string> completedThisSession = new HashSet<string>();
    private bool completionRecorded;
    private bool sessionRestored;
    public event Action<PuzzleNotification> PuzzleCompleted;
    public UnityEvent<string> OnPuzzleIdCompleted => puzzleIdCompleted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void BeginPlaySession() => completedThisSession.Clear();

    public void Initialize(RoomOrganizationPuzzle owner, LolaThoughtPanel panel)
    {
        Unsubscribe();
        puzzle = owner;
        thoughtPanel = panel;
        if (isActiveAndEnabled) Subscribe();
    }

    private void OnEnable() => Subscribe();
    private void OnDisable() => Unsubscribe();
    private void Subscribe()
    {
        if (puzzle == null) return;
        puzzle.Completed -= HandlePuzzleCompleted;
        puzzle.Completed += HandlePuzzleCompleted;
    }
    private void Unsubscribe() { if (puzzle != null) puzzle.Completed -= HandlePuzzleCompleted; }

    private void Start() => RestorePreviousCompletion();

    private void RestorePreviousCompletion()
    {
        if (sessionRestored || puzzle == null || !puzzle.IsReady) return;
        sessionRestored = true;
        if (completedThisSession.Contains(puzzleId))
        {
            completionRecorded = true;
            puzzle.RestoreSolvedLayout();
        }
    }

    private void Update()
    {
        RestorePreviousCompletion();
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame) ShowResult();
    }

    public void ShowResult()
    {
        if (puzzle != null && puzzle.IsSolved && thoughtPanel != null) thoughtPanel.Show(CompletionMessage);
    }

    private void HandlePuzzleCompleted()
    {
        if (completionRecorded || !completedThisSession.Add(puzzleId)) return;
        completionRecorded = true;
        ShowResult();
        var notification = new PuzzleNotification
        {
            PuzzleId = puzzleId,
            Message = CompletionMessage,
            DiscoveredLetters = new[] { "E" }
        };
        PuzzleCompleted?.Invoke(notification);
        puzzleIdCompleted.Invoke(puzzleId);
    }
}
