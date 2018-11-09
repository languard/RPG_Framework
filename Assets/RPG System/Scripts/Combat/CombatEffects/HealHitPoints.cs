using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHitPoints : CombatEffectBase {
    
    public HealHitPoints(ActorCombatController source, ActorCombatController target, string healingExpression)
        :base(source, target, healingExpression, Effect.HealHitPoints)
    {
    }

    public override void ApplyEffect(float healing)
    {
        // TODO: Evaluate healingExpression
        target.entity.hitPoints += (int)healing;
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
