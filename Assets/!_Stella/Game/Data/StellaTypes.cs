using System;
using UnityEngine;

/// <summary>
/// 플레이어가 선택하거나 해금할 수 있는 캐릭터 식별자입니다.
/// </summary>
public enum CharacterId
{
    AriesPrototype,
    Aries,
    Taurus,
    Gemini,
    Cancer,
    Leo,
    Virgo,
    Libra,
    Scorpio,
    Sagittarius,
    Capricorn,
    Aquarius,
    Pisces
}

/// <summary>
/// 캐릭터와 보스 보너스 계산에 사용하는 황도 12궁입니다.
/// </summary>
public enum ZodiacSign
{
    Aries,
    Taurus,
    Gemini,
    Cancer,
    Leo,
    Virgo,
    Libra,
    Scorpio,
    Sagittarius,
    Capricorn,
    Aquarius,
    Pisces
}

/// <summary>
/// 캐릭터와 성장 보너스가 합산되는 기본 전투 스탯입니다.
/// </summary>
[Serializable]
public struct StatBlock
{
    public int maxHealth;
    public int attackPower;
    public int defense;
    public float moveSpeed;
    public float jumpForce;
    public float dashDistance;
    public float dashCooldown;

    /// <summary>
    /// 초기 회색박스 루프에서 사용하는 기본 플레이어 능력치입니다.
    /// </summary>
    public static StatBlock DefaultPlayer()
    {
        return new StatBlock
        {
            maxHealth = 100,
            attackPower = 20,
            defense = 0,
            moveSpeed = 5f,
            jumpForce = 6f,
            dashDistance = 5f,
            dashCooldown = 1.2f
        };
    }
}

/// <summary>
/// 캐릭터 기본 공격의 최소 정의입니다.
/// </summary>
[Serializable]
public struct AttackDefinition
{
    public int damage;
    public float range;
    public float cooldown;

    /// <summary>
    /// 기본 좌클릭 공격값입니다.
    /// </summary>
    public static AttackDefinition Default()
    {
        return new AttackDefinition
        {
            damage = 20,
            range = 3f,
            cooldown = 0.35f
        };
    }
}

/// <summary>
/// 캐릭터 선택과 해금 UI에 쓰는 캐릭터 정의입니다.
/// </summary>
[CreateAssetMenu(menuName = "Stella/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public CharacterId characterId;
    public string displayName;
    public ZodiacSign zodiacSign;
    public bool unlockedByDefault;
    public StatBlock baseStats = StatBlock.DefaultPlayer();
    public AttackDefinition baseAttack = AttackDefinition.Default();
    public CharacterId unlockBossId;
}
