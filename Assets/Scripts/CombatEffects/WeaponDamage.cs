using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : CombatEffectBase {

    private int damage;

    public WeaponDamage(ActorCombatController target, int damage)
        :base(target, damage.ToString())
    {
        this.damage = damage;
    }

    public override void Process()
    {
        target.actor.hitPoints -= damage;
    }
}
