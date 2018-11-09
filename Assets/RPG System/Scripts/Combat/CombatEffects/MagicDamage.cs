using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDamage : CombatEffectBase {

    public MagicDamage(ActorCombatController source, ActorCombatController target, string damageExpression)
        :base(source, target, damageExpression, Effect.MagicDamage)
    {
    }

    public override void ApplyEffect(int damage)
    {
        // TODO: Calculate damage using effectExpression
        target.entity.hitPoints -= damage;
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
