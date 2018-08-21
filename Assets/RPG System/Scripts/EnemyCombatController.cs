using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : ActorCombatController
{
    public string fightDisplayName = "Attack";

    protected override void OnStart()
    {
        commandNames.Add(CommandBase.Command.Fight, fightDisplayName);
    }

    protected override void OnUpdate()
    {
        if (actor.isDisabled)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    protected override void OnReadyForCommand()
    {
        // Enemies will defer to an action selector, some sort of AI ruleset.
        // Right now it's all Fight.

        ActorCombatController target;
        List<ActorCombatController> foes = Foes();
        int playerCount = foes.Count;
        target = foes[Random.Range(0, playerCount)];
        FightCommand fightCommand = CommandFactory.Instance.GetCommand(CommandBase.Command.Fight, this) as FightCommand;
        fightCommand.AddTarget(target);
        RegisterCommand(fightCommand);

    }
    
    public override void ProcessCommand(CommandBase command)
    {
        // We're told what command we're executing, which includes an updated target set.
        // Not sure what this looks like yet - and we're well past due on the night.
        ResetActor();
    }
}
