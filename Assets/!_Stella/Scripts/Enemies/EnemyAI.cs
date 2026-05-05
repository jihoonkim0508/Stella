using UnityEngine;

/// <summary>
/// 간단한 잡몹 AI입니다.
/// 플레이어를 찾고, 추적하고, 공격 범위 안에 들어오면 PlayerHealth에 데미지를 줍니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Death Settings")]
    [SerializeField] private float destroyDelay = 1.5f;

    private Rigidbody rb;
    private Collider enemyCollider;
    private PlayerHealth targetHealth;

    private int currentHealth;
    private bool isDead;
    private float nextAttackTime;

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        currentHealth = maxHealth;

        rb.freezeRotation = true;
    }

    private void Start()
    {
        FindTargetIfNeeded();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        FindTargetIfNeeded();
        if (target == null) return;

        if (targetHealth != null && targetHealth.IsDead)
        {
            StopMoving();
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        LookAtTarget(toTarget);

        if (distance <= attackRange)
        {
            StopMoving();
            TryAttack();
        }
        else
        {
            MoveToTarget(toTarget.normalized);
        }
    }

    private void FindTargetIfNeeded()
    {
        if (target != null)
        {
            if (targetHealth == null) targetHealth = target.GetComponent<PlayerHealth>();
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        target = player.transform;
        targetHealth = player.GetComponent<PlayerHealth>();
    }

    private void MoveToTarget(Vector3 direction)
    {
        Vector3 velocity = direction * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    private void LookAtTarget(Vector3 toTarget)
    {
        if (toTarget.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime
        );
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (targetHealth == null)
        {
            Debug.LogWarning("Enemy found the player, but PlayerHealth is missing.");
            return;
        }

        targetHealth.TakeDamage(attackDamage);
        Debug.Log($"Enemy attacked player: {attackDamage} damage");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (damage <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"Enemy damaged: -{damage}, HP {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        StopMoving();

        if (enemyCollider != null) enemyCollider.enabled = false;
        rb.isKinematic = true;

        Debug.Log("Enemy died.");
        Destroy(gameObject, destroyDelay);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        detectionRange = Mathf.Max(0.1f, detectionRange);
        attackRange = Mathf.Max(0.1f, attackRange);
        turnSpeed = Mathf.Max(0f, turnSpeed);
        attackDamage = Mathf.Max(0, attackDamage);
        attackCooldown = Mathf.Max(0.01f, attackCooldown);
        destroyDelay = Mathf.Max(0f, destroyDelay);
    }
}