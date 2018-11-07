using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : CombatEffectBase {

    private int damage;

    public WeaponDamage(ActorCombatController source, ActorCombatController target, int damage)
        :base(source, target, damage.ToString(), Effect.WeaponDamage)
    {
        //Use the stats from source.entity and target.entity to do more complex calcuations using stats.
        this.damage = damage + source.entity.strength;
        SetDisplayText(damage.ToString());        
    }

    public override void Process()
    {
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
