using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : Person
{
    protected override int SetAttackWaitTimeSec() { return 7; }

    protected override int SetHealth() { return 75; }

    protected override void PerformAttack()
    {
        Anim.SetTrigger(PersonAnimTriggers.Poison.ToString());
        Invoke("HurlItem", .2f);
        Invoke("HurlItem", .4f);
        Invoke("HurlItem", .6f);
    }

    private void HurlItem()
    {
        SFX.Play(CommonSFX.hurl);
        var projectile = ProjectilePools.instance.GetProjectile(ProjectilePoolName.Poison);
        if (projectile)
            projectile.Init(new Vector3(this.transform.position.x + this.transform.forward.x * .5f, transform.position.y + .5f, transform.position.z + transform.forward.z * .5f), MyParade.EnemyParade, 2);
    }

}