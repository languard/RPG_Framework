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
        // TODO: Evaluate healingExpression
        target.entity.hitPoints += healing;
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
