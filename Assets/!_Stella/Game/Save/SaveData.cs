using System;
using System.Collections.Generic;

/// <summary>
/// 단일 JSON 파일에 저장되는 전체 영구 데이터입니다.
/// </summary>
[Serializable]
public class SaveData
{
    public int version = 1;
    public CharacterId selectedCharacter = CharacterId.AriesPrototype;
    public List<CharacterId> unlockedCharacters = new();
    public int commonLevel = 1;
    public int commonStars;
    public List<CharacterProgress> characterProgress = new();
    public List<CharacterId> defeatedBosses = new();
    public int highestThemeReached = 1;
    public List<int> unlockedThemes = new();
    public SettingsData settings = new();

    /// <summary>
    /// 새 저장 데이터의 기본 해금값을 보정합니다.
    /// </summary>
    public void EnsureDefaults()
    {
        if (!unlockedCharacters.Contains(CharacterId.AriesPrototype))
        {
            unlockedCharacters.Add(CharacterId.AriesPrototype);
        }

        if (!unlockedThemes.Contains(1))
        {
            unlockedThemes.Add(1);
        }

        foreach (CharacterId id in CharacterCatalog.AllCharacters)
        {
            GetCharacterProgress(id);
        }
    }

    /// <summary>
    /// 캐릭터별 성장 데이터를 반환하며 없으면 생성합니다.
    /// </summary>
    public CharacterProgress GetCharacterProgress(CharacterId id)
    {
        CharacterProgress progress = characterProgress.Find(item => item.characterId == id);
        if (progress != null)
        {
            return progress;
        }

        progress = new CharacterProgress
        {
            characterId = id,
            level = 1,
            bossStars = 0
        };
        characterProgress.Add(progress);
        return progress;
    }

    /// <summary>
    /// 캐릭터를 중복 없이 해금하고 새 해금 여부를 반환합니다.
    /// </summary>
    public bool UnlockCharacter(CharacterId id)
    {
        if (unlockedCharacters.Contains(id))
        {
            return false;
        }

        unlockedCharacters.Add(id);
        return true;
    }

    /// <summary>
    /// 처치 보스를 중복 없이 기록합니다.
    /// </summary>
    public bool AddDefeatedBoss(CharacterId bossId)
    {
        if (defeatedBosses.Contains(bossId))
        {
            return false;
        }

        defeatedBosses.Add(bossId);
        return true;
    }
}

/// <summary>
/// 캐릭터별 레벨과 보스 별 보유량입니다.
/// </summary>
[Serializable]
public class CharacterProgress
{
    public CharacterId characterId;
    public int level = 1;
    public int bossStars;
}

/// <summary>
/// 초기 설정 화면에서 저장하는 최소 설정값입니다.
/// </summary>
[Serializable]
public class SettingsData
{
    public float mouseSensitivity = 0.1f;
    public float masterVolume = 1f;
}
