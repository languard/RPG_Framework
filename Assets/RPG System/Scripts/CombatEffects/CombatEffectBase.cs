using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatEffectBase {

    public ActorCombatController target { get; private set; }

    public string displayText { get; private set; }

    public CombatEffectBase(ActorCombatController target, string displayText)
    {
        this.target = target;
        this.displayText = displayText;
    }

    public abstract void Process();

    public abstract bool IsValid { get; }
}
