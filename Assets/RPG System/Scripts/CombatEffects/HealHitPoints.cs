using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHitPoints : CombatEffectBase {

    private int healing;

    public HealHitPoints(ActorCombatController target, int healing)
        :base(target, healing.ToString(), Effect.HealHitPoints)
    {
        this.healing = healing;
    }

    public override void Process()
    {
        target.actor.hitPoints += healing;
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
