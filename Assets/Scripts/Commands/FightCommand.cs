using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Fight command. By default you Fight a selected opponent,
/// but you may also Fight a selected ally. There is no stamina or
/// mana cost for this move. Its delay is 1 second.
/// </summary>
public class FightCommand : CommandBase {

    public FightCommand(ActorCombatController owner)
        : base(owner, 1.0f, 0.0f, 0.0f, new int[] { CommandBase.SELECTED_ENEMY, CommandBase.SELECTED_ALLY }, 0)
    {

    }

}
