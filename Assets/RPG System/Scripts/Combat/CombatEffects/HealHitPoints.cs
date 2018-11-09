﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHitPoints : CombatEffectBase {

    private int healing;

    public HealHitPoints(ActorCombatController source, ActorCombatController target, int healing)
        :base(source, target, healing.ToString(), Effect.HealHitPoints)
    {
        this.healing = healing;
    }

    public override void Process()
    {
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