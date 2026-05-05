using UnityEngine;

/// <summary>
/// 원거리형/보스가 사용하는 기본 투사체입니다.
/// Projectile 프리팹에는 Rigidbody + Trigger Collider가 필요합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private int damage;
    private GameObject owner;
    private string targetTag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Collider projectileCollider = GetComponent<Collider>();
        projectileCollider.isTrigger = true;
    }

    public void Initialize(Vector3 direction, float speed, int damage, GameObject owner, string targetTag = "Player")
    {
        this.damage = damage;
        this.owner = owner;
        this.targetTag = targetTag;

        direction = direction.sqrMagnitude <= 0.001f ? transform.forward : direction.normalized;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = direction * speed;
#else
        rb.velocity = direction * speed;
#endif

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.gameObject == owner) return;
        if (owner != null && other.transform.IsChildOf(owner.transform)) return;

        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
