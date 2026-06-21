using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

/// <summary>
/// 각 씬이 로드될 때 초기 플레이어블 루프에 필요한 런타임 오브젝트를 생성합니다.
/// </summary>
public class RuntimeSceneBootstrap : MonoBehaviour
{
    private static bool isRegistered;
    private static GameObject pausePanel;
    private bool resultCommitted;

    /// <summary>
    /// 플레이 시작 전 씬 로드 이벤트와 전역 루트를 준비합니다.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        if (isRegistered)
        {
            return;
        }

        isRegistered = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureGameRoot();
    }

    /// <summary>
    /// 현재 씬 이름에 맞는 UI와 회색박스 스테이지를 구성합니다.
    /// </summary>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureGameRoot();
        EnsureEventSystem();

        if (scene.name == "Boot")
        {
            SceneManager.LoadScene("Start");
            return;
        }

        RuntimeSceneBootstrap bootstrap = FindFirstObjectByType<RuntimeSceneBootstrap>();
        if (bootstrap == null)
        {
            bootstrap = new GameObject("RuntimeSceneBootstrap").AddComponent<RuntimeSceneBootstrap>();
        }

        switch (scene.name)
        {
            case "Start":
                bootstrap.BuildStartScene();
                break;
            case "BattleStage":
                bootstrap.BuildStageScene(StageType.Battle);
                break;
            case "EventStage":
                bootstrap.BuildStageScene(StageType.Event);
                break;
            case "BreakRoom":
                bootstrap.BuildStageScene(StageType.Break);
                break;
            case "BossStage":
                bootstrap.BuildStageScene(StageType.Boss);
                break;
            case "Result":
                bootstrap.BuildResultScene();
                break;
        }
    }

    /// <summary>
    /// 저장, 세션, 스테이지 로더를 가진 영속 루트를 생성합니다.
    /// </summary>
    private static void EnsureGameRoot()
    {
        if (GameSession.Instance != null && StageProgress.Instance != null && StageSceneLoader.Instance != null)
        {
            return;
        }

        GameObject root = GameObject.Find("GameRoot");
        if (root == null)
        {
            root = new GameObject("GameRoot");
            Object.DontDestroyOnLoad(root);
        }

        if (GameSession.Instance == null)
        {
            root.AddComponent<GameSession>();
        }

        if (StageProgress.Instance == null)
        {
            root.AddComponent<StageProgress>();
        }

        if (StageSceneLoader.Instance == null)
        {
            root.AddComponent<StageSceneLoader>();
        }
    }

    /// <summary>
    /// UGUI 버튼 입력을 위한 EventSystem을 보장합니다.
    /// </summary>
    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }

    /// <summary>
    /// 메인 메뉴와 캐릭터 선택/설정 화면을 생성합니다.
    /// </summary>
    private void BuildStartScene()
    {
        ClearRuntimeObjects();
        EnsureCamera(new Vector3(0f, 2f, -6f), Quaternion.Euler(15f, 0f, 0f));
        Canvas canvas = CreateCanvas("StartCanvas");
        GameObject menu = CreatePanel(canvas.transform, "MainMenu");
        CreateTitle(menu.transform, "Stella");
        CreateButton(menu.transform, "Game Start", () => ShowCharacterSelect(canvas, menu));
        CreateButton(menu.transform, "Settings", () => ShowSettings(canvas, menu));
        CreateButton(menu.transform, "Quit", Application.Quit);
    }

    /// <summary>
    /// 해금된 캐릭터만 선택 가능한 캐릭터 선택 화면을 표시합니다.
    /// </summary>
    private void ShowCharacterSelect(Canvas canvas, GameObject previous)
    {
        previous.SetActive(false);
        GameObject panel = CreatePanel(canvas.transform, "CharacterSelect");
        CreateTitle(panel.transform, "Character Select");

        SaveData save = GameSession.Instance.SaveData;
        foreach (CharacterId id in CharacterCatalog.AllCharacters)
        {
            bool unlocked = save.unlockedCharacters.Contains(id);
            Button button = CreateButton(panel.transform, unlocked ? CharacterCatalog.GetDisplayName(id) : $"{CharacterCatalog.GetDisplayName(id)} (Locked)", () =>
            {
                GameSession.Instance.StartRun(id);
                StageProgress.Instance.StartTheme(StageTheme.Theme1);
                StageSceneLoader.Instance.LoadCurrentStage();
            });
            button.interactable = unlocked;
        }

        CreateButton(panel.transform, "Back", () =>
        {
            Destroy(panel);
            previous.SetActive(true);
        });
    }

    /// <summary>
    /// 최소 설정 화면을 표시합니다.
    /// </summary>
    private void ShowSettings(Canvas canvas, GameObject previous)
    {
        previous.SetActive(false);
        GameObject panel = CreatePanel(canvas.transform, "Settings");
        CreateTitle(panel.transform, "Settings");
        CreateLabel(panel.transform, $"Mouse Sensitivity: {GameSession.Instance.SaveData.settings.mouseSensitivity:0.00}");
        CreateLabel(panel.transform, $"Master Volume: {GameSession.Instance.SaveData.settings.masterVolume:0.00}");
        CreateButton(panel.transform, "Back", () =>
        {
            Destroy(panel);
            previous.SetActive(true);
        });
    }

    /// <summary>
    /// 스테이지 지형, 플레이어, 포탈, 적, HUD를 생성합니다.
    /// </summary>
    private void BuildStageScene(StageType stageType)
    {
        ClearRuntimeObjects();
        EnsureLight();
        StageRoomController room = new GameObject("StageRoomController").AddComponent<StageRoomController>();

        CreateFloor(stageType);
        CreatePlayer();
        StagePortal portal = CreatePortal(stageType == StageType.Event || stageType == StageType.Break);
        room.SetPortal(portal);

        if (stageType == StageType.Battle)
        {
            SpawnEnemy(room, EnemyKind.Melee, new Vector3(-4f, 1f, 4f));
            SpawnEnemy(room, EnemyKind.Tanker, new Vector3(0f, 1f, 6f));
            SpawnEnemy(room, EnemyKind.Ranged, new Vector3(4f, 1f, 4f));
        }
        else if (stageType == StageType.Boss)
        {
            CharacterId[] bosses = { CharacterId.Leo, CharacterId.Sagittarius, CharacterId.Capricorn };
            SpawnBoss(room, bosses[Random.Range(0, bosses.Length)], new Vector3(0f, 1.5f, 7f));
        }

        BuildHud(stageType);
        BuildPausePanel();
    }

    /// <summary>
    /// 결과 정산, 성장, 재시작 메뉴를 생성합니다.
    /// </summary>
    private void BuildResultScene()
    {
        ClearRuntimeObjects();
        EnsureCamera(new Vector3(0f, 2f, -6f), Quaternion.Euler(15f, 0f, 0f));
        Canvas canvas = CreateCanvas("ResultCanvas");
        GameObject panel = CreatePanel(canvas.transform, "Result");
        CreateTitle(panel.transform, "Run Result");

        RunSummary summary = resultCommitted ? GameSession.Instance.RunState.ToSummary() : GameSession.Instance.CommitRun();
        resultCommitted = true;

        CreateLabel(panel.transform, $"Common Star +{summary.commonStars}");
        CreateLabel(panel.transform, $"Boss Star +{summary.bossStars}");
        CreateLabel(panel.transform, $"Defeated Bosses: {(summary.defeatedBosses.Count == 0 ? "None" : string.Join(", ", summary.defeatedBosses))}");
        CreateLabel(panel.transform, $"Unlocked: {(summary.newlyUnlockedCharacters.Count == 0 ? "None" : string.Join(", ", summary.newlyUnlockedCharacters))}");
        CreateLabel(panel.transform, $"Reached Theme {summary.reachedTheme}, Room {summary.reachedRoom}");
        CreateButton(panel.transform, "Growth", () => ShowGrowth(canvas, panel));
        CreateButton(panel.transform, "Replay", () => SceneManager.LoadScene("Start"));
    }

    /// <summary>
    /// 공용/캐릭터 성장 버튼을 표시합니다.
    /// </summary>
    private void ShowGrowth(Canvas canvas, GameObject previous)
    {
        previous.SetActive(false);
        GameObject panel = CreatePanel(canvas.transform, "Growth");
        SaveData save = GameSession.Instance.SaveData;
        GrowthService growth = new(save);
        CharacterProgress character = save.GetCharacterProgress(save.selectedCharacter);

        CreateTitle(panel.transform, "Growth");
        CreateLabel(panel.transform, $"Common Lv.{save.commonLevel} / Stars {save.commonStars}");
        CreateButton(panel.transform, $"Upgrade Common ({GrowthService.GetCommonCost(save.commonLevel)})", () =>
        {
            growth.TryUpgradeCommon();
            GameSession.Instance.SaveService.Save();
            Destroy(panel);
            ShowGrowth(canvas, previous);
        });
        CreateLabel(panel.transform, $"{save.selectedCharacter} Lv.{character.level} / Boss Stars {character.bossStars}");
        CreateButton(panel.transform, $"Upgrade Character ({GrowthService.GetCharacterCost(character.level)})", () =>
        {
            growth.TryUpgradeCharacter(save.selectedCharacter);
            GameSession.Instance.SaveService.Save();
            Destroy(panel);
            ShowGrowth(canvas, previous);
        });
        CreateButton(panel.transform, "Back", () =>
        {
            Destroy(panel);
            previous.SetActive(true);
        });
    }

    /// <summary>
    /// Escape 입력으로 일시정지 패널을 토글합니다.
    /// </summary>
    public static void TogglePause()
    {
        if (pausePanel == null)
        {
            return;
        }

        bool paused = !pausePanel.activeSelf;
        pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    private static void ClearRuntimeObjects()
    {
        Time.timeScale = 1f;
        pausePanel = null;
        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            Destroy(canvas.gameObject);
        }

        foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            camera.gameObject.SetActive(false);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RuntimeGenerated"))
        {
            Destroy(obj);
        }
    }

    private static Camera EnsureCamera(Vector3 position, Quaternion rotation)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
        }

        camera.gameObject.tag = "MainCamera";
        camera.transform.SetPositionAndRotation(position, rotation);
        return camera;
    }

    private static void EnsureLight()
    {
        if (FindFirstObjectByType<Light>() != null)
        {
            return;
        }

        GameObject lightObject = new("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static Canvas CreateCanvas(string name)
    {
        GameObject canvasObject = new(name);
        canvasObject.tag = "RuntimeGenerated";
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
        panel.tag = "RuntimeGenerated";
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

    private static Button CreateButton(Transform parent, string text, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new("Button");
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.21f, 0.24f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(action);
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

    private static void CreateFloor(StageType stageType)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.tag = "RuntimeGenerated";
        floor.name = $"{stageType} Floor";
        floor.transform.localScale = new Vector3(18f, 0.25f, 18f);
        floor.transform.position = new Vector3(0f, -0.125f, 4f);
        floor.GetComponent<Renderer>().material.color = stageType switch
        {
            StageType.Event => new Color(0.18f, 0.2f, 0.26f),
            StageType.Break => new Color(0.18f, 0.24f, 0.2f),
            StageType.Boss => new Color(0.24f, 0.18f, 0.2f),
            _ => new Color(0.2f, 0.2f, 0.2f)
        };
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = new("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.1f, -4f);
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.45f;
        controller.center = Vector3.up;
        player.AddComponent<Health>();
        player.AddComponent<Player>();

        GameObject cameraObject = new("PlayerCamera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        cameraObject.AddComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        return player;
    }

    private static StagePortal CreatePortal(bool active)
    {
        GameObject portalObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        portalObject.tag = "RuntimeGenerated";
        portalObject.name = "StagePortal";
        portalObject.transform.position = new Vector3(0f, 1f, 11f);
        portalObject.transform.localScale = new Vector3(1.4f, 0.15f, 1.4f);
        Collider collider = portalObject.GetComponent<Collider>();
        collider.isTrigger = true;
        StagePortal portal = portalObject.AddComponent<StagePortal>();
        portal.SetActive(active);
        portalObject.GetComponent<Renderer>().material.color = Color.cyan;
        return portal;
    }

    private static void SpawnEnemy(StageRoomController room, EnemyKind kind, Vector3 position)
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.tag = "RuntimeGenerated";
        enemy.name = $"{kind}Enemy";
        enemy.transform.position = position;
        enemy.AddComponent<Health>();
        EnemyController controller = enemy.AddComponent<EnemyController>();
        controller.Configure(kind);
        enemy.GetComponent<Renderer>().material.color = kind switch
        {
            EnemyKind.Tanker => Color.gray,
            EnemyKind.Ranged => Color.yellow,
            _ => Color.red
        };
        room.RegisterEnemy(controller);
    }

    private static void SpawnBoss(StageRoomController room, CharacterId bossId, Vector3 position)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boss.tag = "RuntimeGenerated";
        boss.name = $"{bossId}Boss";
        boss.transform.position = position;
        boss.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        boss.AddComponent<Health>();
        EnemyController controller = boss.AddComponent<EnemyController>();
        controller.ConfigureBoss(bossId);
        boss.GetComponent<Renderer>().material.color = bossId switch
        {
            CharacterId.Sagittarius => new Color(0.9f, 0.75f, 0.25f),
            CharacterId.Capricorn => new Color(0.55f, 0.45f, 0.35f),
            _ => new Color(0.95f, 0.5f, 0.2f)
        };
        room.RegisterEnemy(controller);
    }

    private static void BuildHud(StageType stageType)
    {
        Canvas canvas = CreateCanvas("HudCanvas");
        GameObject hud = new("HUD");
        hud.tag = "RuntimeGenerated";
        hud.transform.SetParent(canvas.transform, false);
        RectTransform rect = hud.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 86f);
        HorizontalLayoutGroup layout = hud.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 24f;

        CreateLabel(hud.transform, $"Room: {stageType} {StageProgress.Instance.CurrentRoom}/4");
        CreateLabel(hud.transform, $"Common Star: {GameSession.Instance.RunState.commonStars}");
        CreateLabel(hud.transform, $"Boss Star: {GameSession.Instance.RunState.bossStars}");
    }

    private static void BuildPausePanel()
    {
        Canvas canvas = CreateCanvas("PauseCanvas");
        pausePanel = CreatePanel(canvas.transform, "Pause");
        CreateTitle(pausePanel.transform, "Paused");
        CreateButton(pausePanel.transform, "Continue", TogglePause);
        CreateButton(pausePanel.transform, "Settings", () => { });
        CreateButton(pausePanel.transform, "Main Menu", () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Start");
        });
        pausePanel.SetActive(false);
    }
}
