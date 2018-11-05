using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDamage : CombatEffectBase {

    private int damage;

    public MagicDamage(ActorCombatController target, int damage)
        :base(target, damage.ToString(), Effect.MagicDamage)
    {
        this.damage = damage;
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
