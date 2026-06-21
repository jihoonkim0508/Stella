using System.IO;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 초기 플레이어블 루프의 저장, 성장, 전투 수식을 검증합니다.
/// </summary>
public class StellaCoreTests
{
    /// <summary>
    /// 테스트마다 생성한 Unity 오브젝트를 정리합니다.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        foreach (StageProgress progress in Object.FindObjectsByType<StageProgress>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(progress.gameObject);
        }
    }

    [Test]
    public void SaveDefault_UnlocksAriesPrototypeAndTheme1()
    {
        SaveData saveData = SaveService.CreateDefault();

        Assert.That(saveData.commonLevel, Is.EqualTo(1));
        Assert.That(saveData.unlockedCharacters, Contains.Item(CharacterId.AriesPrototype));
        Assert.That(saveData.unlockedThemes, Contains.Item(1));
    }

    [Test]
    public void SaveService_RoundTripsJson()
    {
        string tempPath = Path.Combine(Path.GetTempPath(), "StellaTests", System.Guid.NewGuid().ToString("N"));
        SaveService saveService = new(tempPath);
        SaveData saveData = saveService.Load();
        saveData.commonStars = 345;
        saveData.UnlockCharacter(CharacterId.Leo);
        saveService.Save();

        SaveService loadedService = new(tempPath);
        SaveData loaded = loadedService.Load();

        Assert.That(loaded.commonStars, Is.EqualTo(345));
        Assert.That(loaded.unlockedCharacters, Contains.Item(CharacterId.Leo));
        Directory.Delete(tempPath, true);
    }

    [Test]
    public void GrowthCosts_FollowDesignFormula()
    {
        Assert.That(GrowthService.GetCommonCost(3), Is.EqualTo(300));
        Assert.That(GrowthService.GetCharacterCost(4), Is.EqualTo(80));
    }

    [Test]
    public void Growth_DoesNotPassMaxLevel()
    {
        SaveData saveData = SaveService.CreateDefault();
        saveData.commonLevel = GrowthService.MaxCommonLevel;
        saveData.commonStars = 99999;
        GrowthService growth = new(saveData);

        Assert.That(growth.TryUpgradeCommon(), Is.False);
        Assert.That(saveData.commonLevel, Is.EqualTo(GrowthService.MaxCommonLevel));
    }

    [Test]
    public void UnlockCharacter_DoesNotDuplicate()
    {
        SaveData saveData = SaveService.CreateDefault();

        Assert.That(saveData.UnlockCharacter(CharacterId.Leo), Is.True);
        Assert.That(saveData.UnlockCharacter(CharacterId.Leo), Is.False);
        Assert.That(saveData.unlockedCharacters.FindAll(id => id == CharacterId.Leo).Count, Is.EqualTo(1));
    }

    [Test]
    public void SameSignBonus_AppliesCeilingTwentyPercent()
    {
        int reward = CharacterCatalog.ApplySameSignBonus(10, CharacterId.Leo, CharacterId.Leo);

        Assert.That(reward, Is.EqualTo(12));
    }

    [Test]
    public void DefenseDamage_MinimumOne()
    {
        Assert.That(CombatMath.ApplyDefense(10, 3), Is.EqualTo(7));
        Assert.That(CombatMath.ApplyDefense(5, 99), Is.EqualTo(1));
    }

    [Test]
    public void Health_DiesOnlyAfterNoHealthRemains()
    {
        GameObject target = new("HealthTarget");
        Health health = target.AddComponent<Health>();
        health.Configure(20, 0);
        bool died = false;
        health.Died += _ => died = true;

        health.TakeDamage(new DamageInfo(null, 10));
        Assert.That(died, Is.False);

        health.TakeDamage(new DamageInfo(null, 10));
        Assert.That(died, Is.True);
        Object.DestroyImmediate(target);
    }

    [Test]
    public void RevivalUnlocks_AtCommonLevelTen()
    {
        Assert.That(GrowthService.HasRevival(9), Is.False);
        Assert.That(GrowthService.HasRevival(10), Is.True);
    }

    [Test]
    public void Theme1Route_ProgressesBattleEventBreakBossThenFinished()
    {
        StageProgress progress = new GameObject("StageProgress").AddComponent<StageProgress>();
        progress.StartTheme(StageTheme.Theme1);

        Assert.That(progress.GetCurrentStageType(), Is.EqualTo(StageType.Battle));
        progress.MoveNext();
        Assert.That(progress.GetCurrentStageType(), Is.EqualTo(StageType.Event));
        progress.MoveNext();
        Assert.That(progress.GetCurrentStageType(), Is.EqualTo(StageType.Break));
        progress.MoveNext();
        Assert.That(progress.GetCurrentStageType(), Is.EqualTo(StageType.Boss));
        progress.MoveNext();
        Assert.That(progress.IsFinished(), Is.True);
    }
}
