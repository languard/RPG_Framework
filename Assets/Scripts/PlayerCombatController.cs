using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : ActorCombatController
{



    private Animator anim; //The parent animator.
    
    private PlayerBattleUIController playerBattleUIController;
   
    // Use this for initialization
    protected override void OnStart() {
        anim = GetComponent<Animator>();
        playerBattleUIController = GameObject.FindGameObjectWithTag("PlayerBattleUIController").GetComponent<PlayerBattleUIController>();

        anim.SetFloat("moveX", -1f);
        anim.SetBool("isMoving", false);
        anim.SetBool("isRunning", false);

        commandNames.Add(CommandBase.Command.Fight, "Fight");
    }

    protected override void OnReadyForCommand()
    {
        // Why does the battleController need to know about this?
        // I guess some kind of controller needs to know which player character
        // is active. Is that really the battle controller though?
        // In the interest of keeping it clean, let's do a UI controller.
        playerBattleUIController.PlayerUnitReadyToSelectCommand(this);
    }
    
    public Dictionary<string, CommandBase.Command> AvailableCommands()
    {
        // For now, only Fight is available.
        Dictionary<string, CommandBase.Command> playerCommands = new Dictionary<string, CommandBase.Command>();
        playerCommands.Add("Fight", CommandBase.Command.Fight );
        return playerCommands;
    }

    public override void ProcessCommand(CommandBase command)
    {
        // We're told what command we're executing, which includes an updated target set.
        // Not sure what this looks like yet - and we're well past due on the night.
        ResetActor();
    }

    protected override void OnUpdate()
    {
        if (actor.isDisabled)
        {
            anim.SetTrigger("onKilled");
        }
    }
}
