using UnityEngine;

/// <summary>
/// 공격자가 대상에게 전달하는 피해 정보입니다.
/// </summary>
public struct DamageInfo
{
    public GameObject source;
    public int amount;

    public DamageInfo(GameObject source, int amount)
    {
        this.source = source;
        this.amount = amount;
    }
}

/// <summary>
/// 피해를 받을 수 있는 대상이 구현하는 인터페이스입니다.
/// </summary>
public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
}

/// <summary>
/// 방어력과 보상 계산에서 공유하는 전투 수식입니다.
/// </summary>
public static class CombatMath
{
    /// <summary>
    /// 방어력 적용 후 피해는 최소 1입니다.
    /// </summary>
    public static int ApplyDefense(int incomingDamage, int defense)
    {
        return Mathf.Max(1, incomingDamage - defense);
    }
}
