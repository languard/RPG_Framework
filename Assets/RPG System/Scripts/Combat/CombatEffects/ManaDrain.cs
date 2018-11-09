using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDrain : CombatEffectBase {

    private int drain;

    public ManaDrain(ActorCombatController source, ActorCombatController target, int drain)
        :base(source, target, string.Empty, Effect.ManaDrain)
    {
        this.drain = drain;
    }

    public override void ApplyEffect(float drain)
    {
        target.entity.mana -= (int)drain;
    }

    public override bool IsValid
    {
        get
        {
            return target.entity.mana >= drain;
        }
    }
}
