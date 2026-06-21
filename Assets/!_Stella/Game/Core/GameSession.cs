using UnityEngine;

/// <summary>
/// 현재 실행 중인 게임 세션의 저장 서비스와 런 상태를 보관합니다.
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public SaveService SaveService { get; private set; }
    public SaveData SaveData => SaveService.Current;
    public RunState RunState { get; private set; } = new();

    /// <summary>
    /// 중복 세션을 제거하고 저장 데이터를 준비합니다.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SaveService = new SaveService();
        SaveService.Load();
    }

    /// <summary>
    /// 테스트에서 세션 오브젝트를 제거할 때 싱글턴 참조를 정리합니다.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 선택 캐릭터로 새 런을 시작합니다.
    /// </summary>
    public void StartRun(CharacterId characterId)
    {
        SaveData.selectedCharacter = characterId;
        SaveService.Save();
        RunState.Reset(characterId);
    }

    /// <summary>
    /// 보스 보상과 해금 결과를 런 상태에만 누적합니다.
    /// </summary>
    public void AddBossClear(CharacterId bossId)
    {
        int bossReward = CharacterCatalog.ApplySameSignBonus(10, RunState.selectedCharacter, bossId);
        RunState.commonStars += 100;
        RunState.bossStars += bossReward;

        if (!RunState.defeatedBosses.Contains(bossId))
        {
            RunState.defeatedBosses.Add(bossId);
        }

        CharacterId unlock = CharacterCatalog.GetUnlockForBoss(bossId);
        if (!SaveData.unlockedCharacters.Contains(unlock) && !RunState.newlyUnlockedCharacters.Contains(unlock))
        {
            RunState.newlyUnlockedCharacters.Add(unlock);
        }
    }

    /// <summary>
    /// 결과 화면에서 런 재화와 해금 상태를 영구 저장에 반영합니다.
    /// </summary>
    public RunSummary CommitRun()
    {
        RunSummary summary = RunState.ToSummary();
        SaveData.commonStars += summary.commonStars;
        CharacterProgress selected = SaveData.GetCharacterProgress(RunState.selectedCharacter);
        selected.bossStars += summary.bossStars;

        foreach (CharacterId bossId in summary.defeatedBosses)
        {
            SaveData.AddDefeatedBoss(bossId);
        }

        foreach (CharacterId characterId in summary.newlyUnlockedCharacters)
        {
            SaveData.UnlockCharacter(characterId);
        }

        SaveData.highestThemeReached = Mathf.Max(SaveData.highestThemeReached, summary.reachedTheme);
        if (summary.defeatedBosses.Count > 0 && !SaveData.unlockedThemes.Contains(2))
        {
            SaveData.unlockedThemes.Add(2);
        }

        SaveService.Save();
        return summary;
    }
}
