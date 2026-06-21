using System;
using System.Collections.Generic;

/// <summary>
/// 초기 구현에서 ScriptableObject 자산 없이 사용할 수 있는 캐릭터 메타데이터 카탈로그입니다.
/// </summary>
public static class CharacterCatalog
{
    private static readonly Dictionary<CharacterId, ZodiacSign> Signs = new()
    {
        { CharacterId.AriesPrototype, ZodiacSign.Aries },
        { CharacterId.Aries, ZodiacSign.Aries },
        { CharacterId.Taurus, ZodiacSign.Taurus },
        { CharacterId.Gemini, ZodiacSign.Gemini },
        { CharacterId.Cancer, ZodiacSign.Cancer },
        { CharacterId.Leo, ZodiacSign.Leo },
        { CharacterId.Virgo, ZodiacSign.Virgo },
        { CharacterId.Libra, ZodiacSign.Libra },
        { CharacterId.Scorpio, ZodiacSign.Scorpio },
        { CharacterId.Sagittarius, ZodiacSign.Sagittarius },
        { CharacterId.Capricorn, ZodiacSign.Capricorn },
        { CharacterId.Aquarius, ZodiacSign.Aquarius },
        { CharacterId.Pisces, ZodiacSign.Pisces }
    };

    /// <summary>
    /// 선택 화면 표시용 캐릭터 목록입니다.
    /// </summary>
    public static readonly CharacterId[] AllCharacters =
    {
        CharacterId.AriesPrototype,
        CharacterId.Aries,
        CharacterId.Taurus,
        CharacterId.Gemini,
        CharacterId.Cancer,
        CharacterId.Leo,
        CharacterId.Virgo,
        CharacterId.Libra,
        CharacterId.Scorpio,
        CharacterId.Sagittarius,
        CharacterId.Capricorn,
        CharacterId.Aquarius,
        CharacterId.Pisces
    };

    /// <summary>
    /// 현재 캐릭터의 별자리를 반환합니다.
    /// </summary>
    public static ZodiacSign GetSign(CharacterId id)
    {
        return Signs[id];
    }

    /// <summary>
    /// 캐릭터 표시명을 반환합니다.
    /// </summary>
    public static string GetDisplayName(CharacterId id)
    {
        return id == CharacterId.AriesPrototype ? "Aries Prototype" : id.ToString();
    }

    /// <summary>
    /// 보스 처치로 해금되는 캐릭터를 반환합니다.
    /// </summary>
    public static CharacterId GetUnlockForBoss(CharacterId bossId)
    {
        return bossId switch
        {
            CharacterId.Leo => CharacterId.Leo,
            CharacterId.Sagittarius => CharacterId.Sagittarius,
            CharacterId.Capricorn => CharacterId.Capricorn,
            _ => throw new ArgumentOutOfRangeException(nameof(bossId), bossId, "해금 대상이 없는 보스입니다.")
        };
    }

    /// <summary>
    /// 선택 캐릭터와 보스 별자리가 같으면 1.2배 보상을 올림으로 적용합니다.
    /// </summary>
    public static int ApplySameSignBonus(int baseReward, CharacterId selectedCharacter, CharacterId bossId)
    {
        return GetSign(selectedCharacter) == GetSign(bossId)
            ? (int)Math.Ceiling(baseReward * 1.2f)
            : baseReward;
    }
}
