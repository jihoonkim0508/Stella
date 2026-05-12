using UnityEngine;

public class StageProgressManager : MonoBehaviour
{
    private const string CurrentStageXKey = "Stella.CurrentStage.X";
    private const string CurrentStageYKey = "Stella.CurrentStage.Y";
    private const string BattleClearsKey = "Stella.BattleClears";
    private const string SelectedModeKey = "Stella.SelectedMode";
    private const string SelectedThemeKey = "Stella.SelectedTheme";

    public static StageProgressManager Instance { get; private set; }

    [SerializeField] private Stage currentStage = new Stage(1, 1);

    public Stage CurrentStage => currentStage;
    public int BattleClears { get; private set; }
    public string SelectedMode { get; private set; }
    public string SelectedTheme { get; private set; }

    public static StageProgressManager EnsureExists()
    {
        if (Instance != null) return Instance;

        var gameObject = new GameObject(nameof(StageProgressManager));
        return gameObject.AddComponent<StageProgressManager>();
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
        Load();
    }

    public void StartNewRun(int stageX)
    {
        currentStage.Set(stageX, 1);
        BattleClears = 0;
        SelectedMode = string.Empty;
        SelectedTheme = string.Empty;
        Save();
    }

    public void SetSelectedMode(string modeName)
    {
        SelectedMode = modeName ?? string.Empty;
        Save();
    }

    public void SetSelectedTheme(string themeName)
    {
        SelectedTheme = themeName ?? string.Empty;
        Save();
    }

    public void SetBattleClears(int clears)
    {
        BattleClears = Mathf.Max(0, clears);
        Save();
    }

    public void AdvanceStage()
    {
        currentStage++;
        BattleClears = 0;
        Save();
    }

    public void Load()
    {
        if (currentStage == null)
        {
            currentStage = new Stage(1, 1);
        }

        int stageX = PlayerPrefs.GetInt(CurrentStageXKey, 1);
        int stageY = PlayerPrefs.GetInt(CurrentStageYKey, 1);
        currentStage.Set(stageX, stageY);

        BattleClears = PlayerPrefs.GetInt(BattleClearsKey, 0);
        SelectedMode = PlayerPrefs.GetString(SelectedModeKey, string.Empty);
        SelectedTheme = PlayerPrefs.GetString(SelectedThemeKey, string.Empty);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(CurrentStageXKey, currentStage.X);
        PlayerPrefs.SetInt(CurrentStageYKey, currentStage.Y);
        PlayerPrefs.SetInt(BattleClearsKey, BattleClears);
        PlayerPrefs.SetString(SelectedModeKey, SelectedMode);
        PlayerPrefs.SetString(SelectedThemeKey, SelectedTheme);
        PlayerPrefs.Save();
    }

    public void ClearSavedProgress()
    {
        PlayerPrefs.DeleteKey(CurrentStageXKey);
        PlayerPrefs.DeleteKey(CurrentStageYKey);
        PlayerPrefs.DeleteKey(BattleClearsKey);
        PlayerPrefs.DeleteKey(SelectedModeKey);
        PlayerPrefs.DeleteKey(SelectedThemeKey);
        Load();
    }
}
