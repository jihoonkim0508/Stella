using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public const string StartScene = "Start";
    public const string LobbyScene = "Lobby";
    public const string RestRoomScene = "RestRoom";
    public const string BattleScene = "Battle";
    public const string BossScene = "Boss";

    public static GameFlowManager Instance { get; private set; }

    [SerializeField] private int requiredBattleClears = 3;
    [SerializeField] private bool returnToRestRoomBetweenBattles;

    private int battleClears;
    private string selectedMode;
    private string selectedTheme;

    public int BattleClears => battleClears;
    public int RequiredBattleClears => requiredBattleClears;
    public string SelectedMode => selectedMode;
    public string SelectedTheme => selectedTheme;

    public static GameFlowManager EnsureExists()
    {
        if (Instance != null) return Instance;

        var gameObject = new GameObject(nameof(GameFlowManager));
        return gameObject.AddComponent<GameFlowManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        requiredBattleClears = Mathf.Max(1, requiredBattleClears);
        StageProgressManager.EnsureExists();
    }

    public void ResetRun()
    {
        battleClears = 0;
        selectedMode = string.Empty;
        selectedTheme = string.Empty;
        StageProgressManager.EnsureExists().StartNewRun(1);
    }

    public void SelectMode(string modeName)
    {
        selectedMode = modeName ?? string.Empty;
        StageProgressManager.EnsureExists().SetSelectedMode(selectedMode);
    }

    public void SelectTheme(string themeName)
    {
        selectedTheme = themeName ?? string.Empty;
        StageProgressManager.EnsureExists().SetSelectedTheme(selectedTheme);
        GoToRestRoom();
    }

    public void GoToLobby()
    {
        ResetRun();
        LoadScene(LobbyScene);
    }

    public void GoToRestRoom()
    {
        LoadScene(RestRoomScene);
    }

    public void EnterBattleFromRestRoom()
    {
        LoadScene(BattleScene);
    }

    public void EnterBattleFromRestRoom(string spawnPointId)
    {
        LoadScene(BattleScene, spawnPointId);
    }

    public void CompleteBattle()
    {
        battleClears++;
        StageProgressManager.EnsureExists().SetBattleClears(battleClears);

        if (battleClears >= requiredBattleClears)
        {
            LoadScene(BossScene);
            return;
        }

        LoadScene(returnToRestRoomBetweenBattles ? RestRoomScene : BattleScene);
    }

    private void LoadScene(string sceneName)
    {
        // LoadScene(sceneName, SpawnPoint.DefaultId);
    }

    private void LoadScene(string sceneName, string spawnPointId)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnValidate()
    {
        requiredBattleClears = Mathf.Max(1, requiredBattleClears);
    }
}
