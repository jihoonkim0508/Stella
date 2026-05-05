using UnityEngine;

public abstract class BossBase : MonoBehaviour, IBoss
{
    [Header("Basic Info")]
    [SerializeField] protected string bossName;
    
    [Header("Health Settings")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    
    [Header("Phase Settings")]
    [Tooltip("2페이즈로 넘어가는 체력 비율 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] protected float phase2Threshold = 0.5f;
    
    protected int currentPhase = 1;

    public string BossName => bossName;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{bossName} HP: {currentHealth} / {maxHealth}");

        // 페이즈 체크
        CheckPhaseTransition();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void CheckPhaseTransition()
    {
        // 현재 1페이즈이고, 체력이 설정된 임계값(Threshold) 이하로 떨어졌을 때
        if (currentPhase == 1 && currentHealth <= maxHealth * phase2Threshold)
        {
            ChangePhase(2);
        }
    }

    public virtual void ChangePhase(int phaseIndex)
    {
        currentPhase = phaseIndex;
        Debug.Log($"{bossName} Phase Changed to: {phaseIndex}");
        OnPhaseChanged(phaseIndex);
    }

    protected abstract void OnPhaseChanged(int phaseIndex);
    
    public abstract void Die();
    protected abstract void ExecutePattern();
}
