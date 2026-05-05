using System.Collections;
using UnityEngine;

/// <summary>
/// 모든 적 타입이 공유하는 기본 기능입니다.
/// - 플레이어 탐색
/// - 체력 / 사망
/// - 보호막
/// - 이동 보조 함수
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Target")]
    [SerializeField] protected Transform target;
    [SerializeField] protected string playerTag = "Player";

    [Header("Base Health")]
    [SerializeField] protected int maxHealth = 50;

    [Header("Base Movement")]
    [SerializeField] protected float turnSpeed = 10f;

    [Header("Death")]
    [SerializeField] protected float destroyDelay = 1.5f;

    protected Rigidbody rb;
    protected Collider enemyCollider;
    protected IDamageable targetDamageable;

    protected int currentHealth;
    protected bool isDead;

    private int shieldHealth;
    private Coroutine shieldRoutine;
    private GameObject activeShieldVisual;

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool HasShield => shieldHealth > 0;
    public int ShieldHealth => shieldHealth;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    protected virtual void Start()
    {
        FindTargetIfNeeded();
    }

    protected void FindTargetIfNeeded()
    {
        if (target != null)
        {
            if (targetDamageable == null)
            {
                targetDamageable = target.GetComponent<IDamageable>();
            }
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        target = player.transform;
        targetDamageable = player.GetComponent<IDamageable>();
    }

    protected bool HasValidTarget()
    {
        if (target == null) return false;
        if (targetDamageable == null) return true; // 체력 스크립트가 없어도 추적은 가능하게 둠
        return !targetDamageable.IsDead;
    }

    protected Vector3 GetFlatDirectionToTarget()
    {
        if (target == null) return Vector3.zero;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        return direction.sqrMagnitude <= 0.001f ? Vector3.zero : direction.normalized;
    }

    protected float GetFlatDistanceToTarget()
    {
        if (target == null) return float.MaxValue;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        return direction.magnitude;
    }

    protected void LookAtTarget()
    {
        Vector3 direction = GetFlatDirectionToTarget();
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
    }

    protected void MoveTowardTarget(float moveSpeed)
    {
        Vector3 direction = GetFlatDirectionToTarget();
        if (direction == Vector3.zero)
        {
            StopMovement();
            return;
        }

        SetHorizontalVelocity(direction * moveSpeed);
    }

    protected void MoveAwayFromTarget(float moveSpeed)
    {
        Vector3 direction = -GetFlatDirectionToTarget();
        if (direction == Vector3.zero)
        {
            StopMovement();
            return;
        }

        SetHorizontalVelocity(direction * moveSpeed);
    }

    protected void StopMovement()
    {
        if (rb == null) return;

        Vector3 velocity = GetVelocity();
        SetVelocity(new Vector3(0f, velocity.y, 0f));
    }

    protected void SetHorizontalVelocity(Vector3 horizontalVelocity)
    {
        if (rb == null) return;

        Vector3 velocity = GetVelocity();
        SetVelocity(new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z));
    }

    protected Vector3 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    protected void SetVelocity(Vector3 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }

    protected void DamageTarget(int damage)
    {
        if (targetDamageable == null)
        {
            Debug.LogWarning($"{name}: 타겟에 IDamageable 또는 PlayerHealth가 없습니다.");
            return;
        }

        targetDamageable.TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        if (damage <= 0) return;

        int remainingDamage = damage;

        if (shieldHealth > 0)
        {
            int absorbed = Mathf.Min(shieldHealth, remainingDamage);
            shieldHealth -= absorbed;
            remainingDamage -= absorbed;

            Debug.Log($"{name}: 보호막이 {absorbed} 데미지를 흡수했습니다. 남은 보호막: {shieldHealth}");

            if (shieldHealth <= 0)
            {
                ClearShield();
            }
        }

        if (remainingDamage <= 0) return;

        currentHealth = Mathf.Max(currentHealth - remainingDamage, 0);
        Debug.Log($"{name}: -{remainingDamage} damage, HP {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual bool ApplyShield(int amount, float duration, GameObject shieldVisualPrefab = null)
    {
        if (isDead) return false;
        if (amount <= 0) return false;
        if (shieldHealth > 0) return false; // 중복 보호막 방지

        shieldHealth = amount;

        if (shieldVisualPrefab != null)
        {
            activeShieldVisual = Instantiate(shieldVisualPrefab, transform);
            activeShieldVisual.transform.localPosition = Vector3.zero;
        }

        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
        }

        shieldRoutine = StartCoroutine(ShieldDurationRoutine(duration));
        Debug.Log($"{name}: 보호막 생성. Shield HP {shieldHealth}");
        return true;
    }

    private IEnumerator ShieldDurationRoutine(float duration)
    {
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }

        ClearShield();
    }

    protected void ClearShield()
    {
        shieldHealth = 0;

        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
            shieldRoutine = null;
        }

        if (activeShieldVisual != null)
        {
            Destroy(activeShieldVisual);
            activeShieldVisual = null;
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        ClearShield();
        StopMovement();

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        OnDied();
        Destroy(gameObject, destroyDelay);
    }

    protected virtual void OnDied()
    {
        Debug.Log($"{name}: died.");
    }

    protected virtual void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        turnSpeed = Mathf.Max(0f, turnSpeed);
        destroyDelay = Mathf.Max(0f, destroyDelay);
    }
}
