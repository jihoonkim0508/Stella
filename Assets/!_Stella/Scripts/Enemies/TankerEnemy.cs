using UnityEngine;

/// <summary>
/// 탱커형 몬스터 예시입니다.
/// - 체력이 높고 공격력은 낮게 세팅하는 타입
/// - 플레이어가 일정 범위 안에 들어오면 주변 몬스터에게 보호막을 1회 부여
/// - 보호막은 중복 적용되지 않음
/// </summary>
public class TankerEnemy : EnemyBase
{
    [Header("Tanker Move")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float moveSpeed = 2.2f;

    [Header("Tanker Attack")]
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Shield Ability")]
    [SerializeField] private float shieldActivateRange = 8f;
    [SerializeField] private float shieldSearchRadius = 7f;
    [SerializeField] private int shieldAmount = 25;
    [SerializeField] private float shieldDuration = 8f;
    [SerializeField] private LayerMask enemyLayerMask = ~0;
    [SerializeField] private GameObject shieldVisualPrefab;
    [SerializeField] private bool includeSelfShield = false;

    private bool hasUsedShieldAbility;
    private float nextAttackTime;

    protected override void Awake()
    {
        base.Awake();

        // 탱커 기본 느낌: 다른 몬스터보다 높은 체력.
        // Inspector에서 maxHealth를 더 크게 조절하면 됩니다.
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        FindTargetIfNeeded();
        if (!HasValidTarget())
        {
            StopMovement();
            return;
        }

        float distance = GetFlatDistanceToTarget();
        if (distance > detectionRange)
        {
            StopMovement();
            return;
        }

        LookAtTarget();
        TryActivateShieldAbility(distance);

        if (distance <= attackRange)
        {
            StopMovement();
            TryMeleeAttack();
        }
        else
        {
            MoveTowardTarget(moveSpeed);
        }
    }

    private void TryActivateShieldAbility(float distanceToPlayer)
    {
        if (hasUsedShieldAbility) return;
        if (distanceToPlayer > shieldActivateRange) return;

        hasUsedShieldAbility = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, shieldSearchRadius, enemyLayerMask);
        int shieldedCount = 0;

        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponentInParent<EnemyBase>();
            if (enemy == null) continue;
            if (enemy.IsDead) continue;
            if (!includeSelfShield && enemy == this) continue;

            bool applied = enemy.ApplyShield(shieldAmount, shieldDuration, shieldVisualPrefab);
            if (applied)
            {
                shieldedCount++;
            }
        }

        Debug.Log($"{name}: 보호막 스킬 발동. 적용된 몬스터 수: {shieldedCount}");
    }

    private void TryMeleeAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        DamageTarget(attackDamage);
        Debug.Log($"{name}: 탱커 근접 공격");
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        detectionRange = Mathf.Max(0.1f, detectionRange);
        attackRange = Mathf.Max(0.1f, attackRange);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        attackDamage = Mathf.Max(0, attackDamage);
        attackCooldown = Mathf.Max(0.01f, attackCooldown);
        shieldActivateRange = Mathf.Max(0.1f, shieldActivateRange);
        shieldSearchRadius = Mathf.Max(0.1f, shieldSearchRadius);
        shieldAmount = Mathf.Max(0, shieldAmount);
        shieldDuration = Mathf.Max(0f, shieldDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, shieldActivateRange);
        Gizmos.DrawWireSphere(transform.position, shieldSearchRadius);
    }
}
