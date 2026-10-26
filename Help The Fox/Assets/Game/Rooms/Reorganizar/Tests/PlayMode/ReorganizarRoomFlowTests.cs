using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ReorganizarRoomFlowTests
{
    private GameObject root;
    private Transform player;
    private TableGrid grid;
    private PushableTable table;
    private RoomOrganizationPuzzle puzzle;
    private LolaThoughtPanel panel;
    private PuzzleCompletionAdapter adapter;
    private RoomExitPortal exit;
    private Keyboard keyboard;
    private float savedTimeScale;
    private string puzzleId;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        savedTimeScale = Time.timeScale;
        Time.timeScale = 1f;
        puzzleId = "ReorganizarTest_" + Guid.NewGuid().ToString("N");
        CreateRoom(false);
        yield return null;
    }

    private void CreateRoom(bool finalPush)
    {
        root = new GameObject("Reorganizar_FlowTest");
        root.SetActive(false);
        grid = root.AddComponent<TableGrid>();
        grid.Configure(new Vector2Int(7, 7), new[] { new Vector2Int(0, 2) });
        puzzle = root.AddComponent<RoomOrganizationPuzzle>();
        player = new GameObject("PlayerPlaceholder_Test").transform;
        player.SetParent(root.transform);
        player.position = new Vector3(1, 2, 0);
        var tableObject = new GameObject("Table_Test");
        tableObject.transform.SetParent(root.transform);
        table = tableObject.AddComponent<PushableTable>();
        table.Initialize(grid, new Vector2Int(2, 2));
        var targetObject = new GameObject("Target_Test");
        targetObject.transform.SetParent(root.transform);
        var target = targetObject.AddComponent<TableTarget>();
        target.Initialize(grid, new Vector2Int(finalPush ? 3 : 4, 2));
        puzzle.Initialize(grid, player, new[] { table }, new[] { target });
        panel = root.AddComponent<LolaThoughtPanel>();
        panel.Initialize(puzzle);
        adapter = root.AddComponent<PuzzleCompletionAdapter>();
        typeof(PuzzleCompletionAdapter).GetField("puzzleId", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(adapter, puzzleId);
        adapter.Initialize(puzzle, panel);
        root.AddComponent<RoomResetController>().Initialize(puzzle);
        var exitObject = new GameObject("Exit_Test");
        exitObject.transform.SetParent(root.transform);
        exitObject.transform.position = player.position;
        exit = exitObject.AddComponent<RoomExitPortal>();
        exit.Initialize(player, puzzle);
        root.SetActive(true);
    }

    private IEnumerator ReplaceRoom(bool finalPush)
    {
        UnityEngine.Object.Destroy(root);
        yield return null;
        CreateRoom(finalPush);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (keyboard != null) InputSystem.RemoveDevice(keyboard);
        if (root != null) UnityEngine.Object.Destroy(root);
        Time.timeScale = savedTimeScale;
        yield return null;
    }

    [UnityTest]
    public IEnumerator CooldownBlocksConsecutivePushesUntilIntervalPasses()
    {
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.False);
        Assert.That(table.GridPosition, Is.EqualTo(new Vector2Int(3, 2)));
        yield return new WaitForSeconds(0.17f);
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        Assert.That(puzzle.IsSolved, Is.True);
    }

    [UnityTest]
    public IEnumerator CompletionWaitsForConfirmationAndCanBeReviewedWithoutAnotherEvent()
    {
        yield return ReplaceRoom(true);
        int completed = 0;
        int sharedCalls = 0;
        adapter.PuzzleCompleted += notification =>
        {
            completed++;
            Assert.That(notification.PuzzleId, Is.EqualTo(puzzleId));
            Assert.That(notification.DiscoveredLetters, Is.EqualTo(new[] { "E" }));
            Assert.That(notification.Message, Is.EqualTo(PuzzleCompletionAdapter.CompletionMessage));
        };
        adapter.OnPuzzleIdCompleted.AddListener(id => sharedCalls++);
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        Assert.That(panel.IsOpen, Is.True);
        Assert.That(puzzle.InputBlocked, Is.True);
        Assert.That(Time.timeScale, Is.EqualTo(1f));
        yield return new WaitForSeconds(0.25f);
        Assert.That(panel.IsOpen, Is.True, "O pensamento não deve fechar sozinho.");
        Assert.That(puzzle.TryMovePlayer(Vector2Int.up), Is.False);
        panel.Close();
        Assert.That(puzzle.TryMovePlayer(Vector2Int.up), Is.True);
        adapter.ShowResult();
        Assert.That(panel.IsOpen, Is.True);
        Assert.That(completed, Is.EqualTo(1));
        Assert.That(sharedCalls, Is.EqualTo(1));
    }

    [UnityTest]
    public IEnumerator ResetRestoresVisualTablesAndPlayerAndEmitsOnlyLocalReset()
    {
        int resets = 0;
        int completions = 0;
        puzzle.PuzzleReset += () => resets++;
        adapter.PuzzleCompleted += _ => completions++;
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        puzzle.ResetPuzzle();
        Assert.That(table.GridPosition, Is.EqualTo(new Vector2Int(2, 2)));
        Assert.That(table.transform.position, Is.EqualTo(new Vector3(2, 2, 0)));
        Assert.That(player.position, Is.EqualTo(new Vector3(1, 2, 0)));
        Assert.That(resets, Is.EqualTo(1));
        Assert.That(completions, Is.EqualTo(0));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RKeyRestoresRoomThroughInputSystem()
    {
        keyboard = InputSystem.AddDevice<Keyboard>();
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.R));
        yield return null;
        yield return null;
        Assert.That(table.GridPosition, Is.EqualTo(new Vector2Int(2, 2)));
        Assert.That(player.position, Is.EqualTo(new Vector3(1, 2, 0)));
    }

    [UnityTest]
    public IEnumerator EnterKeyClosesThoughtAndReturnsControl()
    {
        keyboard = InputSystem.AddDevice<Keyboard>();
        panel.Show(PuzzleCompletionAdapter.CompletionMessage);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
        yield return null;
        yield return null;
        Assert.That(panel.IsOpen, Is.False);
        Assert.That(puzzle.InputBlocked, Is.False);
    }

    [UnityTest]
    public IEnumerator PortalRequiresProximityAndDoesNotRunWhileThoughtIsOpen()
    {
        int returns = 0;
        int inspectorReturns = 0;
        exit.ReturnRequested += () => returns++;
        exit.OnReturnRequested.AddListener(() => inspectorReturns++);
        player.position = new Vector3(5, 5, 0);
        exit.RequestReturn();
        Assert.That(returns, Is.EqualTo(0));
        player.position = exit.transform.position;
        panel.Show("Teste local");
        exit.RequestReturn();
        Assert.That(returns, Is.EqualTo(0));
        panel.Close();
        exit.RequestReturn();
        Assert.That(returns, Is.EqualTo(1));
        Assert.That(inspectorReturns, Is.EqualTo(1));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RecreatedRoomRestoresSolvedTablesWithoutRepeatingNotification()
    {
        yield return ReplaceRoom(true);
        Assert.That(puzzle.TryMovePlayer(Vector2Int.right), Is.True);
        panel.Close();
        yield return ReplaceRoom(true);
        int repeated = 0;
        adapter.PuzzleCompleted += _ => repeated++;
        Assert.That(puzzle.IsSolved, Is.True);
        Assert.That(table.GridPosition, Is.EqualTo(new Vector2Int(3, 2)));
        Assert.That(panel.IsOpen, Is.False);
        puzzle.ResetPuzzle();
        Assert.That(puzzle.IsSolved, Is.True);
        Assert.That(table.GridPosition, Is.EqualTo(new Vector2Int(3, 2)));
        adapter.ShowResult();
        Assert.That(panel.IsOpen, Is.True);
        Assert.That(repeated, Is.EqualTo(0));
    }

    [UnityTest]
    public IEnumerator TableRegistrationMovesBetweenGridsWithoutGhostOccupancy()
    {
        var newGridObject = new GameObject("OtherGrid");
        newGridObject.transform.SetParent(root.transform);
        var newGrid = newGridObject.AddComponent<TableGrid>();
        newGrid.Configure(new Vector2Int(7, 7), Array.Empty<Vector2Int>());
        Assert.That(grid.GetTableAt(new Vector2Int(2, 2)), Is.SameAs(table));
        table.SetTableGrid(newGrid);
        Assert.That(grid.GetTableAt(new Vector2Int(2, 2)), Is.Null);
        Assert.That(newGrid.GetTableAt(new Vector2Int(2, 2)), Is.SameAs(table));
        yield return null;
    }

#if UNITY_EDITOR
    [UnityTest]
    public IEnumerator SavedSceneBuildsRoomWithOriginalSpritesAndSafeSpawn()
    {
        const string scenePath = "Assets/Game/Rooms/Reorganizar/Scenes/SalaReorganizacao.unity";
        Scene scene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scenePath,
            new LoadSceneParameters(LoadSceneMode.Additive));
        yield return null;
        try
        {
            var bootstrap = scene.GetRootGameObjects()[0].GetComponent<ReorganizarRoomBootstrap>();
            Assert.That(bootstrap, Is.Not.Null);
            var scenePuzzle = bootstrap.GetComponentInChildren<RoomOrganizationPuzzle>();
            Assert.That(scenePuzzle.IsReady, Is.True);
            Assert.That(bootstrap.GetComponentsInChildren<PushableTable>().Length, Is.EqualTo(11));
            Assert.That(bootstrap.GetComponentsInChildren<TableTarget>().Length, Is.EqualTo(11));
            var scenePlayer = bootstrap.GetComponentInChildren<RoomPlayerPlaceholder>();
            Assert.That(scenePlayer.transform.position, Is.EqualTo(new Vector3(1, 4, 0)));
            Assert.That(scenePlayer.GetComponent<SpriteRenderer>().sprite.name, Is.EqualTo("front_idle_0"));
            Assert.That(bootstrap.GetComponentInChildren<PushableTable>().GetComponent<SpriteRenderer>().sprite.name,
                Is.EqualTo("desk_0"));
            Assert.That(bootstrap.GetComponentInChildren<Camera>(), Is.Not.Null);
        }
        finally
        {
            SceneManager.UnloadSceneAsync(scene);
        }
        yield return null;
    }
#endif
}
