using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatEffectBase {

    public enum Effect
    {
        WeaponDamage,
        MagicDamage,
        ManaDrain,
        HealHitPoints
    }

    public Effect effectType { get; private set; }

    public ActorCombatController target { get; private set; }

    public string displayText { get; private set; }

    public CombatEffectBase(ActorCombatController target, string displayText, Effect effectType)
    {
        this.target = target;
        this.displayText = displayText;
        this.effectType = effectType;
    }

    public abstract void Process();

    public abstract bool IsValid { get; }
}
