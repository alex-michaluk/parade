using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Person
{

    protected override int SetAttackWaitTimeSec() { return 16; }

    protected override int SetHealth() { return 80; }

    protected override void PerformAttack()
    {
        Anim.SetTrigger(PersonAnimTriggers.Incant.ToString());
        SFX.Play(CommonSFX.strange);
        Invoke("Magic", .6f);
    }


    private void Magic()
    {
        var projectile = ProjectilePools.instance.GetProjectile(ProjectilePoolName.Magic);
        if (projectile)
            projectile.Init(new Vector3(this.transform.position.x + this.transform.forward.x * .5f, transform.position.y + .5f, transform.position.z + transform.forward.z * .5f), MyParade.EnemyParade, 0,true,50);
    }


}