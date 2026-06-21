using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Stella 기본 프리팹과 씬 하이어라키를 생성하는 일회성 에디터 도구입니다.
/// </summary>
public static class StellaSceneBuilder
{
    private const string SceneRoot = "Assets/!_Stella/Scenes";
    private const string PrefabRoot = "Assets/!_Stella/Prefabs";

    /// <summary>
    /// 명령줄 실행용 진입점입니다.
    /// </summary>
    [MenuItem("Stella/Rebuild Scene Hierarchy")]
    public static void RebuildSceneHierarchy()
    {
        EnsureFolders();
        CreatePrefabs();
        BuildStartScene();
        BuildLobbyScene();
        BuildStageScene("BattleStage", StageType.Battle);
        BuildStageScene("EventStage", StageType.Event);
        BuildStageScene("BreakRoom", StageType.Break);
        BuildStageScene("BossStage", StageType.Boss);
        BuildResultScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 프리팹 저장에 필요한 폴더를 보장합니다.
    /// </summary>
    private static void EnsureFolders()
    {
        EnsureFolder("Assets/!_Stella/Prefabs/Player");
        EnsureFolder("Assets/!_Stella/Prefabs/Enemies");
        EnsureFolder("Assets/!_Stella/Prefabs/UI");
        EnsureFolder("Assets/!_Stella/Prefabs/Stages");
    }

    /// <summary>
    /// 기본 GameRoot, Player, 적, 포탈 프리팹을 생성합니다.
    /// </summary>
    private static void CreatePrefabs()
    {
        GameObject gameRoot = new("GameRoot");
        gameRoot.AddComponent<GameSession>();
        gameRoot.AddComponent<StageProgress>();
        gameRoot.AddComponent<StageSceneLoader>();
        SavePrefab(gameRoot, $"{PrefabRoot}/Stages/GameRoot.prefab");

        GameObject player = new("Player");
        player.tag = "Player";
        CharacterController characterController = player.AddComponent<CharacterController>();
        characterController.height = 2f;
        characterController.radius = 0.45f;
        characterController.center = Vector3.up;
        player.AddComponent<Health>();
        Player playerComponent = player.AddComponent<Player>();
        GameObject camera = new("PlayerCamera");
        camera.tag = "MainCamera";
        camera.transform.SetParent(player.transform, false);
        camera.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        Camera cameraComponent = camera.AddComponent<Camera>();
        SetObject(playerComponent, "playerCamera", cameraComponent);
        SavePrefab(player, $"{PrefabRoot}/Player/Player.prefab");

        CreateEnemyPrefab("MeleeEnemy", EnemyKind.Melee);
        CreateEnemyPrefab("TankerEnemy", EnemyKind.Tanker);
        CreateEnemyPrefab("RangedEnemy", EnemyKind.Ranged);
        CreateBossPrefab("LeoBoss", CharacterId.Leo);

        GameObject portal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        portal.name = "StagePortal";
        portal.transform.localScale = new Vector3(1.4f, 0.15f, 1.4f);
        portal.GetComponent<Collider>().isTrigger = true;
        portal.AddComponent<StagePortal>();
        portal.GetComponent<Renderer>().sharedMaterial = CreateMaterial("Portal_Mat", Color.cyan);
        SavePrefab(portal, $"{PrefabRoot}/Stages/StagePortal.prefab");
    }

    /// <summary>
    /// 적 프리팹을 생성하고 종류별 기본값을 적용합니다.
    /// </summary>
    private static void CreateEnemyPrefab(string name, EnemyKind kind)
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = name;
        enemy.AddComponent<Health>();
        EnemyController controller = enemy.AddComponent<EnemyController>();
        controller.Configure(kind);
        enemy.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{name}_Mat", kind switch
        {
            EnemyKind.Tanker => Color.gray,
            EnemyKind.Ranged => Color.yellow,
            _ => Color.red
        });
        SavePrefab(enemy, $"{PrefabRoot}/Enemies/{name}.prefab");
    }

    /// <summary>
    /// 보스 프리팹을 생성합니다.
    /// </summary>
    private static void CreateBossPrefab(string name, CharacterId bossId)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boss.name = name;
        boss.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        boss.AddComponent<Health>();
        EnemyController controller = boss.AddComponent<EnemyController>();
        controller.ConfigureBoss(bossId);
        boss.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{name}_Mat", new Color(0.95f, 0.5f, 0.2f));
        SavePrefab(boss, $"{PrefabRoot}/Enemies/{name}.prefab");
    }

    /// <summary>
    /// 시작 씬 UI를 실제 Canvas와 Button으로 구성합니다.
    /// </summary>
    private static void BuildStartScene()
    {
        Scene scene = OpenAndClear($"{SceneRoot}/Start.unity");
        GameObject systems = CreateRoot("_Systems");
        GameObject ui = CreateRoot("_UI");
        GameObject sceneRoot = CreateRoot("_Scene");

        InstantiatePrefab($"{PrefabRoot}/Stages/GameRoot.prefab", systems.transform);
        CreateEventSystem(systems.transform);
        CreateCamera(sceneRoot.transform, new Vector3(0f, 2f, -6f), Quaternion.Euler(15f, 0f, 0f));

        Canvas canvas = CreateCanvas(ui.transform, "StartCanvas");
        MainMenuController menu = canvas.gameObject.AddComponent<MainMenuController>();
        GameObject mainPanel = CreatePanel(canvas.transform, "MainMenu");
        CreateTitle(mainPanel.transform, "Stella");
        Button ingame = CreateButton(mainPanel.transform, "Ingame");
        Button character = CreateButton(mainPanel.transform, "Character Select");
        Button settings = CreateButton(mainPanel.transform, "Settings");
        Button quit = CreateButton(mainPanel.transform, "Quit");

        GameObject characterPanel = CreatePanel(canvas.transform, "CharacterSelect");
        CreateTitle(characterPanel.transform, "Character Select");
        Transform characterList = CreatePanel(characterPanel.transform, "CharacterList").transform;
        Button template = CreateButton(characterList, "Character");
        Button characterBack = CreateButton(characterPanel.transform, "Back");
        CharacterSelectController select = characterPanel.AddComponent<CharacterSelectController>();

        GameObject settingsPanel = CreatePanel(canvas.transform, "Settings");
        CreateTitle(settingsPanel.transform, "Settings");
        CreateLabel(settingsPanel.transform, "Mouse Sensitivity: 0.10");
        CreateLabel(settingsPanel.transform, "Master Volume: 1.00");
        Button settingsBack = CreateButton(settingsPanel.transform, "Back");

        SetObject(menu, "ingameButton", ingame);
        SetObject(menu, "characterSelectButton", character);
        SetObject(menu, "settingsButton", settings);
        SetObject(menu, "quitButton", quit);
        SetObject(menu, "settingsBackButton", settingsBack);
        SetObject(menu, "mainPanel", mainPanel);
        SetObject(menu, "characterSelectPanel", characterPanel);
        SetObject(menu, "settingsPanel", settingsPanel);
        SetObject(select, "buttonRoot", characterList);
        SetObject(select, "buttonTemplate", template);
        SetObject(select, "backButton", characterBack);
        SetObject(select, "mainMenu", menu);

        SaveScene(scene);
    }

    /// <summary>
    /// 로비 씬에 환경, 플레이어, 테마 문, HUD를 배치합니다.
    /// </summary>
    private static void BuildLobbyScene()
    {
        Scene scene = OpenAndClear($"{SceneRoot}/Lobby.unity");
        GameObject systems = CreateRoot("_Systems");
        GameObject environment = CreateRoot("_Environment");
        GameObject gameplay = CreateRoot("_Gameplay");
        GameObject ui = CreateRoot("_UI");

        InstantiatePrefab($"{PrefabRoot}/Stages/GameRoot.prefab", systems.transform);
        CreateEventSystem(systems.transform);
        CreateLight(environment.transform);
        CreateFloor(environment.transform, "Lobby Floor", new Vector3(22f, 0.25f, 18f), new Vector3(0f, -0.125f, 3f), new Color(0.16f, 0.18f, 0.2f));
        CreateFloor(environment.transform, "Lobby Door Wall", new Vector3(22f, 4f, 0.35f), new Vector3(0f, 2f, 9.25f), new Color(0.11f, 0.12f, 0.13f));
        InstantiatePrefab($"{PrefabRoot}/Player/Player.prefab", gameplay.transform).transform.position = new Vector3(0f, 1.1f, -4f);

        for (int themeNumber = 1; themeNumber <= 5; themeNumber++)
        {
            CreateThemeDoor(gameplay.transform, themeNumber, new Vector3((themeNumber - 3) * 3.2f, 1.5f, 8f));
        }

        Canvas canvas = CreateCanvas(ui.transform, "LobbyCanvas");
        GameObject hud = CreateTopBar(canvas.transform, "LobbyHUD");
        LobbyHudController lobbyHud = hud.AddComponent<LobbyHudController>();
        CreateLabel(hud.transform, "Lobby");
        TextMeshProUGUI characterLabel = CreateLabel(hud.transform, "Character:");
        TextMeshProUGUI themesLabel = CreateLabel(hud.transform, "Unlocked Themes:");
        SetObject(lobbyHud, "characterLabel", characterLabel);
        SetObject(lobbyHud, "unlockedThemesLabel", themesLabel);

        SaveScene(scene);
    }

    /// <summary>
    /// 전투/이벤트/휴식/보스 씬을 공통 규칙으로 구성합니다.
    /// </summary>
    private static void BuildStageScene(string sceneName, StageType stageType)
    {
        Scene scene = OpenAndClear($"{SceneRoot}/{sceneName}.unity");
        GameObject systems = CreateRoot("_Systems");
        GameObject environment = CreateRoot("_Environment");
        GameObject gameplay = CreateRoot("_Gameplay");
        GameObject spawns = CreateRoot("_Spawns");
        GameObject ui = CreateRoot("_UI");

        InstantiatePrefab($"{PrefabRoot}/Stages/GameRoot.prefab", systems.transform);
        CreateEventSystem(systems.transform);
        CreateLight(environment.transform);
        CreateFloor(environment.transform, $"{stageType} Floor", new Vector3(18f, 0.25f, 18f), new Vector3(0f, -0.125f, 4f), StageColor(stageType));

        GameObject playerSpawn = new("PlayerSpawn");
        playerSpawn.transform.SetParent(spawns.transform);
        playerSpawn.transform.position = new Vector3(0f, 1.1f, -4f);
        InstantiatePrefab($"{PrefabRoot}/Player/Player.prefab", gameplay.transform).transform.position = playerSpawn.transform.position;

        StageRoomController room = new GameObject("StageRoomController").AddComponent<StageRoomController>();
        room.transform.SetParent(gameplay.transform);
        StagePortal portal = InstantiatePrefab($"{PrefabRoot}/Stages/StagePortal.prefab", gameplay.transform).GetComponent<StagePortal>();
        portal.transform.position = new Vector3(0f, 1f, 11f);
        room.SetPortal(portal);
        SetBool(room, "clearOnStart", stageType == StageType.Event || stageType == StageType.Break);

        List<EnemyController> enemies = new();
        if (stageType == StageType.Battle)
        {
            enemies.Add(PlaceEnemy($"{PrefabRoot}/Enemies/MeleeEnemy.prefab", gameplay.transform, new Vector3(-4f, 1f, 4f), room));
            enemies.Add(PlaceEnemy($"{PrefabRoot}/Enemies/TankerEnemy.prefab", gameplay.transform, new Vector3(0f, 1f, 6f), room));
            enemies.Add(PlaceEnemy($"{PrefabRoot}/Enemies/RangedEnemy.prefab", gameplay.transform, new Vector3(4f, 1f, 4f), room));
        }
        else if (stageType == StageType.Boss)
        {
            enemies.Add(PlaceEnemy($"{PrefabRoot}/Enemies/LeoBoss.prefab", gameplay.transform, new Vector3(0f, 1.5f, 7f), room));
        }

        SetObjectList(room, "enemies", enemies);

        Canvas canvas = CreateCanvas(ui.transform, "RunCanvas");
        GameObject hud = CreateTopBar(canvas.transform, "RunHUD");
        RunHudController runHud = hud.AddComponent<RunHudController>();
        TextMeshProUGUI roomLabel = CreateLabel(hud.transform, "Room:");
        TextMeshProUGUI commonLabel = CreateLabel(hud.transform, "Common Star:");
        TextMeshProUGUI bossLabel = CreateLabel(hud.transform, "Boss Star:");
        SetObject(runHud, "roomLabel", roomLabel);
        SetObject(runHud, "commonStarLabel", commonLabel);
        SetObject(runHud, "bossStarLabel", bossLabel);
        CreatePauseMenu(canvas.transform);

        SaveScene(scene);
    }

    /// <summary>
    /// 결과 씬의 결과/성장 패널을 구성합니다.
    /// </summary>
    private static void BuildResultScene()
    {
        Scene scene = OpenAndClear($"{SceneRoot}/Result.unity");
        GameObject systems = CreateRoot("_Systems");
        GameObject ui = CreateRoot("_UI");
        GameObject sceneRoot = CreateRoot("_Scene");

        InstantiatePrefab($"{PrefabRoot}/Stages/GameRoot.prefab", systems.transform);
        CreateEventSystem(systems.transform);
        CreateCamera(sceneRoot.transform, new Vector3(0f, 2f, -6f), Quaternion.Euler(15f, 0f, 0f));

        Canvas canvas = CreateCanvas(ui.transform, "ResultCanvas");
        ResultController result = canvas.gameObject.AddComponent<ResultController>();
        GameObject resultPanel = CreatePanel(canvas.transform, "Result");
        CreateTitle(resultPanel.transform, "Run Result");
        TextMeshProUGUI common = CreateLabel(resultPanel.transform, "Common Star +0");
        TextMeshProUGUI boss = CreateLabel(resultPanel.transform, "Boss Star +0");
        TextMeshProUGUI defeated = CreateLabel(resultPanel.transform, "Defeated Bosses:");
        TextMeshProUGUI unlocked = CreateLabel(resultPanel.transform, "Unlocked:");
        TextMeshProUGUI reached = CreateLabel(resultPanel.transform, "Reached Theme 1, Room 1");
        Button growthButton = CreateButton(resultPanel.transform, "Growth");
        Button replayButton = CreateButton(resultPanel.transform, "Replay");

        GameObject growthPanel = CreatePanel(canvas.transform, "Growth");
        GrowthController growth = growthPanel.AddComponent<GrowthController>();
        CreateTitle(growthPanel.transform, "Growth");
        TextMeshProUGUI commonGrowth = CreateLabel(growthPanel.transform, "Common");
        Button commonUpgrade = CreateButton(growthPanel.transform, "Upgrade Common");
        TextMeshProUGUI characterGrowth = CreateLabel(growthPanel.transform, "Character");
        Button characterUpgrade = CreateButton(growthPanel.transform, "Upgrade Character");
        Button back = CreateButton(growthPanel.transform, "Back");

        SetObject(result, "commonStarLabel", common);
        SetObject(result, "bossStarLabel", boss);
        SetObject(result, "defeatedBossesLabel", defeated);
        SetObject(result, "unlockedCharactersLabel", unlocked);
        SetObject(result, "reachedStageLabel", reached);
        SetObject(result, "resultPanel", resultPanel);
        SetObject(result, "growthPanel", growthPanel);
        SetObject(result, "growthButton", growthButton);
        SetObject(result, "replayButton", replayButton);
        SetObject(growth, "commonLabel", commonGrowth);
        SetObject(growth, "characterLabel", characterGrowth);
        SetObject(growth, "commonUpgradeButton", commonUpgrade);
        SetObject(growth, "characterUpgradeButton", characterUpgrade);
        SetObject(growth, "backButton", back);
        SetObject(growth, "resultController", result);

        SaveScene(scene);
    }

    private static EnemyController PlaceEnemy(string prefabPath, Transform parent, Vector3 position, StageRoomController room)
    {
        EnemyController enemy = InstantiatePrefab(prefabPath, parent).GetComponent<EnemyController>();
        enemy.transform.position = position;
        enemy.SetRoom(room);
        return enemy;
    }

    private static void CreateThemeDoor(Transform parent, int themeNumber, Vector3 position)
    {
        GameObject door = new($"Theme{themeNumber}Door");
        door.transform.SetParent(parent);
        door.transform.position = position;

        BoxCollider trigger = door.AddComponent<BoxCollider>();
        trigger.size = new Vector3(1.8f, 3f, 0.4f);
        trigger.isTrigger = true;

        GameObject unlockedVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        unlockedVisual.name = "UnlockedVisual";
        unlockedVisual.transform.SetParent(door.transform, false);
        unlockedVisual.transform.localScale = new Vector3(1.8f, 3f, 0.4f);
        unlockedVisual.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"Theme{themeNumber}_Unlocked_Mat", new Color(0.1f, 0.55f, 0.75f));
        Object.DestroyImmediate(unlockedVisual.GetComponent<Collider>());

        GameObject lockedVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lockedVisual.name = "LockedVisual";
        lockedVisual.transform.SetParent(door.transform, false);
        lockedVisual.transform.localScale = new Vector3(1.8f, 3f, 0.4f);
        lockedVisual.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"Theme{themeNumber}_Locked_Mat", new Color(0.2f, 0.2f, 0.2f));
        Object.DestroyImmediate(lockedVisual.GetComponent<Collider>());

        LobbyThemeDoor themeDoor = door.AddComponent<LobbyThemeDoor>();
        SetInt(themeDoor, "themeNumber", themeNumber);
        SetObject(themeDoor, "unlockedVisual", unlockedVisual);
        SetObject(themeDoor, "lockedVisual", lockedVisual);
        SetObject(themeDoor, "triggerCollider", trigger);
    }

    private static void CreatePauseMenu(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "Pause");
        PauseMenuController pause = panel.AddComponent<PauseMenuController>();
        CreateTitle(panel.transform, "Paused");
        Button continueButton = CreateButton(panel.transform, "Continue");
        Button mainMenuButton = CreateButton(panel.transform, "Main Menu");
        SetObject(pause, "panel", panel);
        SetObject(pause, "continueButton", continueButton);
        SetObject(pause, "mainMenuButton", mainMenuButton);
    }

    private static Scene OpenAndClear(string path)
    {
        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Object.DestroyImmediate(root);
        }

        return scene;
    }

    private static void SaveScene(Scene scene)
    {
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject CreateRoot(string name)
    {
        return new GameObject(name);
    }

    private static GameObject InstantiatePrefab(string prefabPath, Transform parent)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent, false);
        return instance;
    }

    private static void SavePrefab(GameObject gameObject, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        Object.DestroyImmediate(gameObject);
    }

    private static void CreateEventSystem(Transform parent)
    {
        GameObject eventSystem = new("EventSystem");
        eventSystem.transform.SetParent(parent);
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }

    private static Camera CreateCamera(Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject cameraObject = new("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.SetPositionAndRotation(position, rotation);
        return cameraObject.AddComponent<Camera>();
    }

    private static void CreateLight(Transform parent)
    {
        GameObject lightObject = new("Directional Light");
        lightObject.transform.SetParent(parent);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static void CreateFloor(Transform parent, string name, Vector3 scale, Vector3 position, Color color)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = name;
        floor.transform.SetParent(parent);
        floor.transform.localScale = scale;
        floor.transform.position = position;
        floor.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{name}_Mat", color);
    }

    private static Canvas CreateCanvas(Transform parent, string name)
    {
        GameObject canvasObject = new(name);
        canvasObject.transform.SetParent(parent);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.35f, 0.15f);
        rect.anchorMax = new Vector2(0.65f, 0.9f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12f;
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        panel.AddComponent<Image>().color = new Color(0.08f, 0.09f, 0.1f, 0.86f);
        return panel;
    }

    private static GameObject CreateTopBar(Transform parent, string name)
    {
        GameObject hud = new(name);
        hud.transform.SetParent(parent, false);
        RectTransform rect = hud.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 86f);
        HorizontalLayoutGroup layout = hud.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 24f;
        return hud;
    }

    private static void CreateTitle(Transform parent, string text)
    {
        TextMeshProUGUI label = CreateLabel(parent, text);
        label.fontSize = 44f;
        label.alignment = TextAlignmentOptions.Center;
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string text)
    {
        GameObject labelObject = new("Text");
        labelObject.transform.SetParent(parent, false);
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 24f;
        label.color = Color.white;
        label.textWrappingMode = TextWrappingModes.Normal;
        LayoutElement layout = labelObject.AddComponent<LayoutElement>();
        layout.minHeight = 42f;
        return label;
    }

    private static Button CreateButton(Transform parent, string text)
    {
        GameObject buttonObject = new("Button");
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.21f, 0.24f);
        Button button = buttonObject.AddComponent<Button>();
        LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
        layout.minHeight = 54f;
        TextMeshProUGUI label = CreateLabel(buttonObject.transform, text);
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 22f;
        RectTransform textRect = label.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        return button;
    }

    private static Color StageColor(StageType stageType)
    {
        return stageType switch
        {
            StageType.Event => new Color(0.18f, 0.2f, 0.26f),
            StageType.Break => new Color(0.18f, 0.24f, 0.2f),
            StageType.Boss => new Color(0.24f, 0.18f, 0.2f),
            _ => new Color(0.2f, 0.2f, 0.2f)
        };
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = $"{PrefabRoot}/Stages/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
        string name = System.IO.Path.GetFileName(path);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    private static void SetObject(Object target, string propertyName, Object value)
    {
        SerializedObject serializedObject = new(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetInt(Object target, string propertyName, int value)
    {
        SerializedObject serializedObject = new(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.intValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetBool(Object target, string propertyName, bool value)
    {
        SerializedObject serializedObject = new(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.boolValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetObjectList<T>(Object target, string propertyName, List<T> values)
        where T : Object
    {
        SerializedObject serializedObject = new(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.arraySize = values.Count;
        for (int i = 0; i < values.Count; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
