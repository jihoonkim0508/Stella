using UnityEngine;

public class TankMob : TrashMob
{
    [SerializeField] private float triggerRange = 20;
    [SerializeField] private float shieldRadius = 20;

    private bool shieldUsed = false;

    protected override void Update()
    {
        if (player == null)
        {
            base.Update();
            return;
        }

        float dist = GetDistanceToPlayer();

        if (!shieldUsed && dist <= triggerRange)
        {
            ApplyShield();
            shieldUsed = true;
        }

        base.Update();
    }

    protected override void Attack()
    {
        player.GetComponent<PlayerController>().TakeDamage(GetAttackPower());
    }

    protected override float GetAttackCooldown()
    {
        return 1.2f;
    }

    private void ApplyShield()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, shieldRadius);

        foreach (var col in cols)
        {
            TrashMob mob = col.GetComponent<TrashMob>();
            if (mob != null && mob != this)
            {
                mob.AddHp(20f);
            }
        }
    }
}