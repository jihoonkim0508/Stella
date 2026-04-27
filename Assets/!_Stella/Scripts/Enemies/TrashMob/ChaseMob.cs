using UnityEngine;

public class ChaseMob : TrashMob
{
    protected override void Attack()
    {
        player.GetComponent<PlayerController>().TakeDamage(GetAttackPower());
    }

    protected override float GetAttackCooldown()
    {
        return 1f;
    }
}