using UnityEngine;

public class RangedMob : TrashMob
{
    [SerializeField] private float detectRange = 40;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Update()
    {
        if (player == null)
        {
            base.Update();
            return;
        }

        float dist = GetDistanceToPlayer();

        if (dist <= detectRange)
        {
            if (dist > GetAttackRange())
                MoveToPlayer();
            else
                base.Update();
        }
    }

    protected override void Attack()
    {
        ShootProjectile();
    }

    private void ShootProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        proj.GetComponent<Rigidbody>().linearVelocity = dir * 10f;
    }
        
    protected override float GetAttackCooldown()
    {
        return 1.5f;
    }

    private float GetAttackRange()
    {
        return typeof(TrashMob)
            .GetField("attackRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(this) is float value ? value : 0f;
    }
}