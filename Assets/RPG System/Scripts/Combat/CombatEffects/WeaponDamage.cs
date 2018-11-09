using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : CombatEffectBase {

    public WeaponDamage(ActorCombatController source, ActorCombatController target, string damageExpression)
        :base(source, target, damageExpression, Effect.WeaponDamage)
    {
    }

    public override void ApplyEffect(float damage)
    {
        // TODO: Evaluate effectExpression to calculate damage
        target.entity.hitPoints -= (int)damage;
    }

    public override bool IsValid
    {
        get
        {
            return true;
        }
    }
}
