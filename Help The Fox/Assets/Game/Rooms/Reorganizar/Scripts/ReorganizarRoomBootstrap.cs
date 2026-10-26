using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cell = TableBoardState.Cell;

// This isolated test scene constructs its own objects at Play time; other scenes remain untouched.
public class ReorganizarRoomBootstrap : MonoBehaviour
{
    [SerializeField] private Sprite tableSprite = null;
    [SerializeField] private Sprite lolaSprite = null;
    [SerializeField] private bool showTargetsDuringPlay = false;
    [Header("Team integration")]
    [SerializeField] private UnityEvent<string> puzzleCompleted = new UnityEvent<string>();
    [SerializeField] private UnityEvent returnRequested = new UnityEvent();
    private Sprite square;
    private Material spriteMaterial;
    private RoomOrganizationPuzzle puzzle;
    private PuzzleCompletionAdapter completion;
    private LolaThoughtPanel thoughtPanel;
    private RoomExitPortal exit;
    private readonly List<GameObject> targetMarks = new List<GameObject>();
    private GUIStyle hudStyle;
    private string exitMessage;

    private void Awake()
    {
        square = Sprite.Create(Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f), Texture2D.whiteTexture.width);
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader != null) spriteMaterial = new Material(shader);

        var room = new GameObject("Reorganizar_Runtime");
        room.transform.SetParent(transform, false);
        room.SetActive(false);
        var grid = room.AddComponent<TableGrid>();
        var walls = new List<Vector2Int>();
        foreach (Cell wall in ReorganizarLayout.CreateWalls()) walls.Add(Vector(wall));
        grid.Configure(new Vector2Int(ReorganizarLayout.Width, ReorganizarLayout.Height), walls);
        puzzle = room.AddComponent<RoomOrganizationPuzzle>();

        for (int x = 0; x < ReorganizarLayout.Width; x++)
            for (int y = 0; y < ReorganizarLayout.Height; y++)
            {
                var cell = new Vector2Int(x, y);
                bool wall = walls.Contains(cell);
                Color color = wall ? new Color(0.15f, 0.21f, 0.26f) :
                    ((x + y) % 2 == 0 ? new Color(0.36f, 0.42f, 0.46f) : new Color(0.39f, 0.45f, 0.49f));
                GameObject tile = Visual((wall ? "Wall_" : "Floor_") + x + "_" + y,
                    room.transform, grid.CellToWorld(cell), square, color, 0.98f, 0);
                if (wall) tile.AddComponent<BoxCollider2D>();
            }

        var targets = new List<TableTarget>();
        foreach (Cell targetCell in ReorganizarLayout.CreateTargets())
        {
            var targetObject = new GameObject("Target_E_" + targetCell.X + "_" + targetCell.Y);
            targetObject.transform.SetParent(room.transform, false);
            var target = targetObject.AddComponent<TableTarget>();
            target.Initialize(grid, Vector(targetCell));
            targets.Add(target);
            var marks = new GameObject("DebugTargetOutline");
            marks.transform.SetParent(targetObject.transform, false);
            Color color = new Color(0.4f, 0.95f, 0.65f);
            for (int side = 0; side < 4; side++)
            {
                bool horizontal = side < 2;
                Vector3 offset = horizontal ? new Vector3(0, side == 0 ? -0.4f : 0.4f, 0) :
                    new Vector3(side == 2 ? -0.4f : 0.4f, 0, 0);
                GameObject mark = Visual("Outline", marks.transform, targetObject.transform.position + offset,
                    square, color, 1f, 1);
                mark.transform.localScale = horizontal ? new Vector3(0.8f, 0.035f, 1) : new Vector3(0.035f, 0.8f, 1);
            }
            marks.SetActive(showTargetsDuringPlay);
            targetMarks.Add(marks);
        }

        var tables = new List<PushableTable>();
        int tableNumber = 1;
        foreach (Cell tableCell in ReorganizarLayout.CreateTables())
        {
            GameObject tableObject = Visual("Table_" + tableNumber++, room.transform,
                grid.CellToWorld(Vector(tableCell)), tableSprite != null ? tableSprite : square,
                tableSprite != null ? Color.white : new Color(0.85f, 0.62f, 0.35f), 0.84f, 2);
            var collider = tableObject.AddComponent<BoxCollider2D>();
            // Collider dimensions are local; compensate for the artwork's fitted scale.
            collider.size = new Vector2(0.84f / tableObject.transform.localScale.x,
                0.84f / tableObject.transform.localScale.y);
            var table = tableObject.AddComponent<PushableTable>();
            table.Initialize(grid, Vector(tableCell));
            tables.Add(table);
        }

        var spawnObject = new GameObject("PlayerSpawn_Safe");
        spawnObject.transform.SetParent(room.transform, false);
        spawnObject.transform.position = grid.CellToWorld(Vector(ReorganizarLayout.Spawn));
        var spawn = spawnObject.AddComponent<RoomSpawnPoint>();
        GameObject player = Visual("PlayerPlaceholder_Lola_Local", room.transform, spawnObject.transform.position,
            lolaSprite != null ? lolaSprite : square,
            lolaSprite != null ? Color.white : new Color(1f, 0.5f, 0.2f), 0.7f, 3);
        spawn.TryPlacePlayer(player.transform, grid);
        var driver = player.AddComponent<RoomPlayerPlaceholder>();
        puzzle.Initialize(grid, player.transform, tables, targets);
        driver.Initialize(puzzle);
        room.AddComponent<RoomResetController>().Initialize(puzzle);
        thoughtPanel = room.AddComponent<LolaThoughtPanel>();
        thoughtPanel.Initialize(puzzle);
        completion = room.AddComponent<PuzzleCompletionAdapter>();
        completion.Initialize(puzzle, thoughtPanel);
        completion.OnPuzzleIdCompleted.AddListener(HandlePuzzleCompleted);

        GameObject door = Visual("ReturnToCorridor_PLACEHOLDER", room.transform,
            grid.CellToWorld(Vector(ReorganizarLayout.Exit)), square, new Color(0.63f, 0.47f, 0.83f), 0.9f, 1);
        exit = door.AddComponent<RoomExitPortal>();
        exit.Initialize(player.transform, puzzle);
        exit.ReturnRequested += HandleReturnRequested;
        room.SetActive(true);

        var cameraObject = new GameObject("ReorganizarCamera");
        cameraObject.transform.SetParent(transform, false);
        var roomCamera = cameraObject.AddComponent<Camera>();
        roomCamera.orthographic = true;
        float aspect = Mathf.Max(0.1f, (float)Screen.width / Mathf.Max(1, Screen.height));
        roomCamera.orthographicSize = Mathf.Max(6.3f, 7.5f / aspect);
        roomCamera.clearFlags = CameraClearFlags.SolidColor;
        roomCamera.backgroundColor = new Color(0.07f, 0.1f, 0.14f);
        cameraObject.transform.position = new Vector3(6.5f, 4.5f, -10f);
        cameraObject.tag = "MainCamera";
    }

    private GameObject Visual(string name, Transform parent, Vector3 position, Sprite sprite,
        Color color, float size, int order)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = position;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = order;
        if (spriteMaterial != null) renderer.sharedMaterial = spriteMaterial;
        float longestSide = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
        obj.transform.localScale = Vector3.one * (size / Mathf.Max(0.01f, longestSide));
        return obj;
    }

    private void HandleReturnRequested()
    {
        exitMessage = "Saída preparada. A conexão com o corredor será feita pela equipe de integração.";
        Debug.Log("Reorganizar: retorno solicitado pelo jogador; conectar RoomExitPortal.OnReturnRequested.", this);
        returnRequested.Invoke();
    }

    private void HandlePuzzleCompleted(string puzzleId) => puzzleCompleted.Invoke(puzzleId);

    private void Update()
    {
        if (puzzle != null && puzzle.IsSolved)
            foreach (GameObject mark in targetMarks) if (!mark.activeSelf) mark.SetActive(true);
    }

    private void OnGUI()
    {
        if (puzzle == null || thoughtPanel == null) return;
        if (hudStyle == null) hudStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        GUI.Box(new Rect(8, 8, Screen.width - 16, 76), GUIContent.none);
        GUI.Label(new Rect(20, 12, Screen.width - 40, 26), "Sala de Reorganização — organize as mesas para revelar a pista.", hudStyle);
        GUI.Label(new Rect(20, 40, Screen.width - 40, 38),
            "WASD / setas: andar e empurrar   |   R: reiniciar   |   E: saída   |   C: consultar pista", hudStyle);

        bool previousEnabled = GUI.enabled;
        GUI.enabled = !thoughtPanel.IsOpen;
        if (puzzle.IsSolved)
        {
            GUI.Label(new Rect(20, Screen.height - 80, 300, 30), "Pista descoberta: E", hudStyle);
            if (GUI.Button(new Rect(Screen.width - 190, Screen.height - 60, 170, 40), "Consultar pista (C)")) completion.ShowResult();
        }
        else if (GUI.Button(new Rect(Screen.width - 190, Screen.height - 60, 170, 40), "Reiniciar sala (R)")) puzzle.ResetPuzzle();

        if (exit.PlayerIsClose && GUI.Button(new Rect(20, Screen.height - 120, 180, 32), "Retornar ao corredor (E)")) exit.RequestReturn();
        GUI.enabled = previousEnabled;
        if (!string.IsNullOrEmpty(exitMessage))
            GUI.Label(new Rect(20, Screen.height - 45, Mathf.Max(120, Screen.width - 230), 42), exitMessage, hudStyle);
    }

    private void OnDestroy()
    {
        if (exit != null) exit.ReturnRequested -= HandleReturnRequested;
        if (completion != null) completion.OnPuzzleIdCompleted.RemoveListener(HandlePuzzleCompleted);
        if (square != null) Destroy(square);
        if (spriteMaterial != null) Destroy(spriteMaterial);
    }

    private static Vector2Int Vector(Cell cell) => new Vector2Int(cell.X, cell.Y);
}
