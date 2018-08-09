using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatEffectBase {

    public enum Target
    {
        Self,
        SelectedAlly,
        AllAllies,
        SelectedOpponent,
        AllOpponents,
        RandomOpponent
    }

    public Target target { get; private set; }

    public CombatEffectBase(Target target)
    {
        this.target = target;
    }

}
