using System;
using System.Collections.Generic;

/// <summary>
/// 런 중에만 누적되는 임시 진행 상태입니다.
/// </summary>
[Serializable]
public class RunState
{
    public CharacterId selectedCharacter = CharacterId.AriesPrototype;
    public int commonStars;
    public int bossStars;
    public List<CharacterId> defeatedBosses = new();
    public List<CharacterId> newlyUnlockedCharacters = new();
    public int reachedTheme = 1;
    public int reachedRoom = 1;
    public bool revivalUsed;

    /// <summary>
    /// 새 런 시작 시 임시 재화를 초기화합니다.
    /// </summary>
    public void Reset(CharacterId characterId)
    {
        selectedCharacter = characterId;
        commonStars = 0;
        bossStars = 0;
        defeatedBosses.Clear();
        newlyUnlockedCharacters.Clear();
        reachedTheme = 1;
        reachedRoom = 1;
        revivalUsed = false;
    }

    /// <summary>
    /// 결과 화면과 저장 정산에 사용할 요약을 만듭니다.
    /// </summary>
    public RunSummary ToSummary()
    {
        return new RunSummary
        {
            commonStars = commonStars,
            bossStars = bossStars,
            defeatedBosses = new List<CharacterId>(defeatedBosses),
            newlyUnlockedCharacters = new List<CharacterId>(newlyUnlockedCharacters),
            reachedTheme = reachedTheme,
            reachedRoom = reachedRoom
        };
    }
}

/// <summary>
/// 런 결과 화면에 표시하고 저장에 반영할 요약 데이터입니다.
/// </summary>
[Serializable]
public class RunSummary
{
    public int commonStars;
    public int bossStars;
    public List<CharacterId> defeatedBosses = new();
    public List<CharacterId> newlyUnlockedCharacters = new();
    public int reachedTheme;
    public int reachedRoom;
}
