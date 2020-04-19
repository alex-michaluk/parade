using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Techie : Person {

    protected override int SetAttackWaitTimeSec() { return 1; }

    protected override int SetHealth() { return 30; } 

    protected override void PerformAttack()
    {
        Anim.SetTrigger(PersonAnimTriggers.Throw.ToString());
        Invoke("HurlItem", .5f);
        SFX.Play(CommonSFX.swing);
    }

    private void HurlItem()
    {
        var projectile = ProjectilePools.instance.GetProjectile(ProjectilePoolName.Techie);
        if (projectile)
            projectile.Init(new Vector3(this.transform.position.x + this.transform.forward.x * .5f, transform.position.y + .5f, transform.position.z + transform.forward.z * .5f), MyParade.EnemyParade,0,true,1);
    }

}
