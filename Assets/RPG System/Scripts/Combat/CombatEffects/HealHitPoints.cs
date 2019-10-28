using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHitPoints : CombatEffectBase {
    
    public HealHitPoints(ActorCombatController source, ActorCombatController target, string healingExpression)
        :base(source, target, healingExpression, Effect.HealHitPoints)
    {
    }

    public override void ApplyEffect(int healing)
    {
        // Negative HP healing not allowed
        if(healing > 0) target.entity.HealHP(healing);
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
