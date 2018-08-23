﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDrain : CombatEffectBase {

    private int drain;

    public ManaDrain(ActorCombatController target, int drain)
        :base(target, string.Empty)
    {
        this.drain = drain;
    }

    public override void Process()
    {
        target.actor.mana -= drain;
    }

    public override bool IsValid
    {
        get
        {
            return target.actor.mana >= drain;
        }
    }
}
