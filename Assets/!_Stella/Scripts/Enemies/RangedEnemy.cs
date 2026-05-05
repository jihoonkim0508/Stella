using UnityEngine;

/// <summary>
/// 원거리형 몬스터 예시입니다.
/// - 플레이어가 감지 범위 안에 들어오면 바라봄
/// - 공격 범위 안이면 투사체 발사
/// - 너무 가까우면 뒤로 빠짐
/// </summary>
public class RangedEnemy : EnemyBase
{
    [Header("Ranged Move")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float keepDistance = 6f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float retreatSpeed = 3.5f;

    [Header("Ranged Attack")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private bool useDirectDamageIfNoProjectile = true;

    private float nextAttackTime;

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

        if (distance < keepDistance)
        {
            MoveAwayFromTarget(retreatSpeed);
        }
        else if (distance > attackRange)
        {
            MoveTowardTarget(moveSpeed);
        }
        else
        {
            StopMovement();
            TryRangedAttack();
        }
    }

    private void TryRangedAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + transform.forward;
        Vector3 direction = GetFlatDirectionToTarget();

        if (projectilePrefab != null)
        {
            EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            projectile.Initialize(direction, projectileSpeed, attackDamage, gameObject, playerTag);
        }
        else if (useDirectDamageIfNoProjectile)
        {
            DamageTarget(attackDamage);
            Debug.Log($"{name}: ProjectilePrefab이 없어서 직접 데미지로 대체했습니다.");
        }

        Debug.Log($"{name}: 원거리 공격");
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        detectionRange = Mathf.Max(0.1f, detectionRange);
        attackRange = Mathf.Max(0.1f, attackRange);
        keepDistance = Mathf.Max(0.1f, keepDistance);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        retreatSpeed = Mathf.Max(0f, retreatSpeed);
        attackDamage = Mathf.Max(0, attackDamage);
        attackCooldown = Mathf.Max(0.01f, attackCooldown);
        projectileSpeed = Mathf.Max(0.1f, projectileSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, keepDistance);
    }
}
