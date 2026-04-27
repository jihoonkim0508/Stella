using UnityEngine;

public abstract class TrashMob : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float attackPower;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;

    protected Transform player;
    private bool isAttacking;

    protected virtual void Start()
    {
        FindPlayer();
    }

    protected virtual void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackRange)
            MoveToPlayer();
        else
            TryAttack();
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    protected void MoveToPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void TryAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        Attack();

        Invoke(nameof(ResetAttack), GetAttackCooldown());
    }

    protected abstract void Attack();

    protected virtual float GetAttackCooldown() => 1f;

    private void ResetAttack()
    {
        isAttacking = false;
    }

    protected float GetAttackPower() => attackPower;

    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    // TankMob 같은 경우 사용
    public void AddHp(float value)
    {
        hp += value;
    }

    protected float GetDistanceToPlayer()
    {
        return player == null ? float.MaxValue :
            Vector3.Distance(transform.position, player.position);
    }
}