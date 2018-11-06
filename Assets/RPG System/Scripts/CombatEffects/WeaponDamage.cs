using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : CombatEffectBase {

    private int damage;

    public WeaponDamage(ActorCombatController source, ActorCombatController target, int damage)
        :base(source, target, damage.ToString(), Effect.WeaponDamage)
    {
        this.damage = damage + source.entity.strength;
        UnityEngine.Debug.Log("Entity name is " + source.name);
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
