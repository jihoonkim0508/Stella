/// <summary>
/// 공용 성장과 캐릭터 성장을 저장 데이터에 적용합니다.
/// </summary>
public class GrowthService
{
    public const int MaxCommonLevel = 15;
    public const int MaxCharacterLevel = 5;

    private readonly SaveData saveData;

    public GrowthService(SaveData saveData)
    {
        this.saveData = saveData;
    }

    /// <summary>
    /// 공용 성장 비용은 현재 레벨 x 100입니다.
    /// </summary>
    public static int GetCommonCost(int currentLevel)
    {
        return currentLevel * 100;
    }

    /// <summary>
    /// 캐릭터 성장 비용은 5 x 현재 레벨 제곱입니다.
    /// </summary>
    public static int GetCharacterCost(int currentLevel)
    {
        return 5 * currentLevel * currentLevel;
    }

    /// <summary>
    /// 공용 별을 소모해 공용 레벨을 올립니다.
    /// </summary>
    public bool TryUpgradeCommon()
    {
        if (saveData.commonLevel >= MaxCommonLevel)
        {
            return false;
        }

        int cost = GetCommonCost(saveData.commonLevel);
        if (saveData.commonStars < cost)
        {
            return false;
        }

        saveData.commonStars -= cost;
        saveData.commonLevel++;
        return true;
    }

    /// <summary>
    /// 보스 별을 소모해 선택 캐릭터 레벨을 올립니다. 스킬 효과는 아직 적용하지 않습니다.
    /// </summary>
    public bool TryUpgradeCharacter(CharacterId characterId)
    {
        CharacterProgress progress = saveData.GetCharacterProgress(characterId);
        if (progress.level >= MaxCharacterLevel)
        {
            return false;
        }

        int cost = GetCharacterCost(progress.level);
        if (progress.bossStars < cost)
        {
            return false;
        }

        progress.bossStars -= cost;
        progress.level++;
        return true;
    }

    /// <summary>
    /// 공용 성장 레벨에 따른 플레이어 스탯 보너스를 적용합니다.
    /// </summary>
    public static StatBlock ApplyCommonBonuses(StatBlock baseStats, int commonLevel)
    {
        int bonusLevels = commonLevel - 1;
        baseStats.attackPower += bonusLevels * 2;
        baseStats.maxHealth += bonusLevels * 10;
        baseStats.moveSpeed += bonusLevels * 0.1f;
        return baseStats;
    }

    /// <summary>
    /// 공용 성장 레벨로 더블 점프 가능 여부를 계산합니다.
    /// </summary>
    public static bool HasDoubleJump(int commonLevel)
    {
        return commonLevel >= 5;
    }

    /// <summary>
    /// 공용 성장 레벨로 1회 부활 가능 여부를 계산합니다.
    /// </summary>
    public static bool HasRevival(int commonLevel)
    {
        return commonLevel >= 10;
    }

    /// <summary>
    /// 공용 성장 레벨로 대쉬 무적 가능 여부를 계산합니다.
    /// </summary>
    public static bool HasDashInvulnerability(int commonLevel)
    {
        return commonLevel >= 15;
    }
}
