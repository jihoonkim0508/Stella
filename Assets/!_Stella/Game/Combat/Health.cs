using System;
using UnityEngine;

/// <summary>
/// 플레이어와 적이 공유하는 체력 및 피해 처리 컴포넌트입니다.
/// </summary>
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int defense;

    public event Action<Health> Died;
    public event Action<Health> Changed;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;
    public bool IsInvulnerable { get; set; }

    /// <summary>
    /// 시작 시 최대 체력으로 초기화합니다.
    /// </summary>
    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// 최대 체력과 방어력을 설정하고 체력을 가득 채웁니다.
    /// </summary>
    public void Configure(int maxHealth, int defense)
    {
        this.maxHealth = Mathf.Max(1, maxHealth);
        this.defense = Mathf.Max(0, defense);
        CurrentHealth = this.maxHealth;
        Changed?.Invoke(this);
    }

    /// <summary>
    /// 방어력을 적용한 피해를 받습니다.
    /// </summary>
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead || IsInvulnerable)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - CombatMath.ApplyDefense(damageInfo.amount, defense));
        Changed?.Invoke(this);

        if (IsDead)
        {
            Died?.Invoke(this);
        }
    }

    /// <summary>
    /// 체력을 지정 비율로 회복합니다.
    /// </summary>
    public void Revive(float ratio)
    {
        CurrentHealth = Mathf.Clamp(Mathf.CeilToInt(maxHealth * ratio), 1, maxHealth);
        Changed?.Invoke(this);
    }
}
