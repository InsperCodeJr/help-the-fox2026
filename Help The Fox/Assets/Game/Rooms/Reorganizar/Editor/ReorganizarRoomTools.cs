using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Editor-only helpers. No automatic solver is included in the published game.
public static class ReorganizarRoomTools
{
    private const string ScenePath = "Assets/Game/Rooms/Reorganizar/Scenes/SalaReorganizacao.unity";
    private const string MenuRoot = "Help The Fox/Reorganizar/";
    private static RoomOrganizationPuzzle puzzle;
    private static RoomPlayerPlaceholder driver;
    private static bool driverWasEnabled;
    private static int nextStep;
    private static float nextAttempt;
    private static bool running;

    [MenuItem(MenuRoot + "Abrir sala de teste", priority = 0)]
    private static void OpenRoom()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem(MenuRoot + "Abrir sala de teste", true)]
    private static bool CanOpenRoom() => !EditorApplication.isPlayingOrWillChangePlaymode;

    [MenuItem(MenuRoot + "Testar solução (reinicia sala)", priority = 10)]
    private static void StartSolution()
    {
        puzzle = Object.FindAnyObjectByType<RoomOrganizationPuzzle>();
        if (puzzle == null || !puzzle.IsReady || puzzle.InputBlocked || puzzle.IsSolved) return;
        puzzle.ResetPuzzle();
        driver = puzzle.GetComponentInChildren<RoomPlayerPlaceholder>();
        driverWasEnabled = driver != null && driver.enabled;
        if (driver != null) driver.enabled = false;
        nextStep = 0;
        nextAttempt = Time.time + 0.05f;
        running = true;
        EditorApplication.update += AdvanceSolution;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        AssemblyReloadEvents.beforeAssemblyReload += StopSolution;
        Debug.Log("Reorganizar: teste automático iniciado. Aguarde a Lola percorrer a solução.", puzzle);
    }

    [MenuItem(MenuRoot + "Testar solução (reinicia sala)", true)]
    private static bool CanStartSolution()
    {
        if (running || !EditorApplication.isPlaying || EditorApplication.isPaused) return false;
        var room = Object.FindAnyObjectByType<RoomOrganizationPuzzle>();
        return room != null && room.IsReady && !room.IsSolved && !room.InputBlocked;
    }

    [MenuItem(MenuRoot + "Parar teste automático", priority = 11)]
    private static void StopSolution()
    {
        EditorApplication.update -= AdvanceSolution;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        AssemblyReloadEvents.beforeAssemblyReload -= StopSolution;
        if (driver != null) driver.enabled = driverWasEnabled;
        driver = null;
        puzzle = null;
        running = false;
    }

    [MenuItem(MenuRoot + "Parar teste automático", true)]
    private static bool CanStopSolution() => running;

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode) StopSolution();
    }

    private static void AdvanceSolution()
    {
        if (puzzle == null || !EditorApplication.isPlaying) { StopSolution(); return; }
        if (EditorApplication.isPaused || Time.timeScale <= 0f) return;
        if (puzzle.IsSolved)
        {
            Debug.Log("Reorganizar: formação E concluída pelos movimentos normais. Confirme o pensamento e use C para consultá-lo outra vez.", puzzle);
            StopSolution();
            return;
        }
        if (puzzle.InputBlocked || Time.time < nextAttempt) return;
        if (nextStep >= ReorganizarLayout.VerifiedSolution.Length)
        {
            Debug.LogError("Reorganizar: a solução terminou sem completar o puzzle. Confira se o layout foi alterado.", puzzle);
            StopSolution();
            return;
        }
        var cell = ReorganizarLayout.Direction(ReorganizarLayout.VerifiedSolution[nextStep]);
        if (!puzzle.TryMovePlayer(new Vector2Int(cell.X, cell.Y)))
        {
            Debug.LogError("Reorganizar: movimento de teste bloqueado na etapa " + (nextStep + 1) +
                ". O teste usa o layout padrão; reinicie-o sem mover manualmente a personagem.", puzzle);
            StopSolution();
            return;
        }
        nextStep++;
        nextAttempt = Time.time + 0.2f;
    }
}
