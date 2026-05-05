using System.Collections;
using UnityEngine;

/// <summary>
/// 패턴형 보스 예시입니다.
/// - 추적형 + 원거리형을 섞은 기본 행동
/// - 일정 주기마다 보스 패턴 실행
/// - 예시 패턴: 돌진 / 연속 투사체 / 원형 탄막
/// </summary>
public class Boss : EnemyBase
{
    private enum BossPattern
    {
        Rush,
        RangedBurst,
        RadialShot
    }

    [Header("Boss Basic Move")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Boss Basic Melee")]
    [SerializeField] private float meleeRange = 2.2f;
    [SerializeField] private int meleeDamage = 15;
    [SerializeField] private float meleeCooldown = 1.2f;

    [Header("Boss Basic Ranged")]
    [SerializeField] private float rangedRange = 12f;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private int rangedDamage = 10;
    [SerializeField] private float rangedCooldown = 2f;
    [SerializeField] private float projectileSpeed = 13f;

    [Header("Pattern Timing")]
    [SerializeField] private float firstPatternDelay = 3f;
    [SerializeField] private float patternCooldown = 6f;
    [SerializeField] private float patternTelegraphTime = 0.6f;

    [Header("Pattern - Rush")]
    [SerializeField] private float rushSpeed = 14f;
    [SerializeField] private float rushDuration = 0.45f;
    [SerializeField] private int rushDamage = 20;

    [Header("Pattern - Ranged Burst")]
    [SerializeField] private int burstShotCount = 5;
    [SerializeField] private float burstShotInterval = 0.18f;

    [Header("Pattern - Radial Shot")]
    [SerializeField] private int radialProjectileCount = 12;
    [SerializeField] private int radialDamage = 8;
    [SerializeField] private float radialProjectileSpeed = 10f;

    private float nextBasicAttackTime;
    private float nextPatternTime;
    private bool isPatternRunning;

    protected override void Start()
    {
        base.Start();
        nextPatternTime = Time.time + firstPatternDelay;
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

        if (!isPatternRunning && Time.time >= nextPatternTime)
        {
            StartCoroutine(RunRandomPatternRoutine());
            return;
        }

        if (isPatternRunning) return;

        if (distance <= meleeRange)
        {
            StopMovement();
            TryMeleeAttack();
        }
        else if (distance <= rangedRange)
        {
            StopMovement();
            TryBasicRangedAttack();
        }
        else
        {
            MoveTowardTarget(moveSpeed);
        }
    }

    private void TryMeleeAttack()
    {
        if (Time.time < nextBasicAttackTime) return;
        nextBasicAttackTime = Time.time + meleeCooldown;

        DamageTarget(meleeDamage);
        Debug.Log($"{name}: 보스 기본 근접 공격");
    }

    private void TryBasicRangedAttack()
    {
        if (Time.time < nextBasicAttackTime) return;
        nextBasicAttackTime = Time.time + rangedCooldown;

        ShootProjectileAtTarget(rangedDamage, projectileSpeed);
        Debug.Log($"{name}: 보스 기본 원거리 공격");
    }

    private IEnumerator RunRandomPatternRoutine()
    {
        isPatternRunning = true;
        StopMovement();

        BossPattern pattern = (BossPattern)Random.Range(0, 3);
        Debug.Log($"{name}: 패턴 준비 - {pattern}");

        // 이 시간에 애니메이션/이펙트/경고 UI를 연결하면 됩니다.
        yield return new WaitForSeconds(patternTelegraphTime);

        switch (pattern)
        {
            case BossPattern.Rush:
                yield return RushPatternRoutine();
                break;
            case BossPattern.RangedBurst:
                yield return RangedBurstPatternRoutine();
                break;
            case BossPattern.RadialShot:
                RadialShotPattern();
                break;
        }

        StopMovement();
        isPatternRunning = false;
        nextPatternTime = Time.time + patternCooldown;
    }

    private IEnumerator RushPatternRoutine()
    {
        Debug.Log($"{name}: 돌진 패턴 시작");

        Vector3 rushDirection = GetFlatDirectionToTarget();
        float endTime = Time.time + rushDuration;
        bool hasHitPlayer = false;

        while (Time.time < endTime)
        {
            SetHorizontalVelocity(rushDirection * rushSpeed);

            if (!hasHitPlayer && GetFlatDistanceToTarget() <= meleeRange)
            {
                hasHitPlayer = true;
                DamageTarget(rushDamage);
            }

            yield return null;
        }
    }

    private IEnumerator RangedBurstPatternRoutine()
    {
        Debug.Log($"{name}: 연속 원거리 패턴 시작");

        for (int i = 0; i < burstShotCount; i++)
        {
            if (!HasValidTarget()) yield break;

            LookAtTarget();
            ShootProjectileAtTarget(rangedDamage, projectileSpeed);
            yield return new WaitForSeconds(burstShotInterval);
        }
    }

    private void RadialShotPattern()
    {
        Debug.Log($"{name}: 원형 탄막 패턴 시작");

        if (radialProjectileCount <= 0) return;

        for (int i = 0; i < radialProjectileCount; i++)
        {
            float angle = 360f / radialProjectileCount * i;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            SpawnProjectile(direction, radialDamage, radialProjectileSpeed);
        }
    }

    private void ShootProjectileAtTarget(int damage, float speed)
    {
        Vector3 direction = GetFlatDirectionToTarget();
        SpawnProjectile(direction, damage, speed);
    }

    private void SpawnProjectile(Vector3 direction, int damage, float speed)
    {
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + direction;

        if (projectilePrefab != null)
        {
            EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            projectile.Initialize(direction, speed, damage, gameObject, playerTag);
        }
        else
        {
            // 프리팹이 없을 때도 기본 원거리 공격 테스트가 가능하도록 보정.
            // 원형 탄막은 프리팹이 있어야 시각적으로 확인됩니다.
            if (Vector3.Dot(direction.normalized, GetFlatDirectionToTarget()) > 0.95f)
            {
                DamageTarget(damage);
            }

            Debug.LogWarning($"{name}: projectilePrefab이 없어 투사체 시각화 없이 처리됩니다.");
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        detectionRange = Mathf.Max(0.1f, detectionRange);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        meleeRange = Mathf.Max(0.1f, meleeRange);
        meleeDamage = Mathf.Max(0, meleeDamage);
        meleeCooldown = Mathf.Max(0.01f, meleeCooldown);
        rangedRange = Mathf.Max(0.1f, rangedRange);
        rangedDamage = Mathf.Max(0, rangedDamage);
        rangedCooldown = Mathf.Max(0.01f, rangedCooldown);
        projectileSpeed = Mathf.Max(0.1f, projectileSpeed);
        firstPatternDelay = Mathf.Max(0f, firstPatternDelay);
        patternCooldown = Mathf.Max(0.1f, patternCooldown);
        patternTelegraphTime = Mathf.Max(0f, patternTelegraphTime);
        rushSpeed = Mathf.Max(0.1f, rushSpeed);
        rushDuration = Mathf.Max(0.01f, rushDuration);
        rushDamage = Mathf.Max(0, rushDamage);
        burstShotCount = Mathf.Max(1, burstShotCount);
        burstShotInterval = Mathf.Max(0.01f, burstShotInterval);
        radialProjectileCount = Mathf.Max(1, radialProjectileCount);
        radialDamage = Mathf.Max(0, radialDamage);
        radialProjectileSpeed = Mathf.Max(0.1f, radialProjectileSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }
}
