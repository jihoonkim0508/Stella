using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어의 체력, 피격, 사망을 담당합니다.
/// Player 오브젝트에 붙이고 maxHealth를 Inspector에서 조절하세요.
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<int, int> { }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitInvincibleTime = 0.25f;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;

    [Header("Events")]
    [SerializeField] private HealthChangedEvent onHealthChanged;
    [SerializeField] private UnityEvent onDied;

    private int currentHealth;
    private bool isDead;
    private float nextDamageableTime;

    private Rigidbody rb;
    private PlayerController playerController;
    private PlayerCombat playerCombat;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();

        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (damage <= 0) return;
        if (Time.time < nextDamageableTime) return;

        nextDamageableTime = Time.time + hitInvincibleTime;
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Player damaged: -{damage}, HP {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("Player died.");

        if (playerController != null) playerController.enabled = false;
        if (playerCombat != null) playerCombat.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        onDied?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        hitInvincibleTime = Mathf.Max(0f, hitInvincibleTime);
        destroyDelay = Mathf.Max(0f, destroyDelay);
    }
}