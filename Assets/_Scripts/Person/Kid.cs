using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid : Person {

    protected override int SetAttackWaitTimeSec() { return 3; }

    protected override int SetHealth() { return 40; }

    protected override void PerformAttack()
    {
        Anim.SetTrigger(PersonAnimTriggers.Throw.ToString());
        Invoke("HurlItem", .5f);
        SFX.Play(CommonSFX.swing);
    }

    private void HurlItem()
    {
        var projectile = ProjectilePools.instance.GetProjectile(ProjectilePoolName.Standard);
        if (projectile)
            projectile.Init(new Vector3(this.transform.position.x + this.transform.forward.x * .5f, transform.position.y + .5f, transform.position.z + transform.forward.z * .5f), MyParade.EnemyParade, 2);
    }

}
