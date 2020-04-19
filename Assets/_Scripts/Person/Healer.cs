using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Person {

    protected override int SetAttackWaitTimeSec() { return 10; }

    protected override int SetHealth() { return 60; }

    protected override void PerformAttack()
    {
        Anim.SetTrigger(PersonAnimTriggers.Heal.ToString());
        SFX.Play(CommonSFX.warp);
        Invoke("HealParade", .6f);
        MyParade.PlayParticlesHealth();
    }

    private void HealParade()
    {
        foreach (Person p in MyParade.People)
            p.Heal(Random.Range(9,15));
        MyParade.RefreshRightText();
    }

}
