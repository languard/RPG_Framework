﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : ActorCombatController
{

    private Animator anim; //The parent animator.
    
    private PlayerBattleUIController playerBattleUIController;

    public PlayerSkills playerSkills;
    public string playerCombatID;

    private GameMaster gm;
   
    // Use this for initialization
    protected override void OnStart() {
        //need to pull Entity from GameMaster
        gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        entity = gm.GetPartyMemberByID(playerCombatID);

        anim = GetComponent<Animator>();
        playerBattleUIController = GameObject.FindGameObjectWithTag("PlayerBattleUIController").GetComponent<PlayerBattleUIController>();
        playerSkills = GetComponent<PlayerSkills>();     
        //playerSkills = entity.playerSkills;
    }

    protected override void OnReadyForCommand()
    {
        // Why does the battleController need to know about this?
        // I guess some kind of controller needs to know which player character
        // is active. Is that really the battle controller though?
        // In the interest of keeping it clean, let's do a UI controller.
        playerBattleUIController.PlayerUnitReadyToSelectCommand(this);
    }

    public override void ProcessCommand(CommandBase command)
    {
        // We're told what command we're executing, which includes an updated target set.
        // Not sure what this looks like yet - and we're well past due on the night.
        ResetActor();
    }

    protected override void OnUpdate()
    {
        if (entity.isDisabled)
        {
            //HACK
            //no animation, just disable sprite
            //anim.SetTrigger("onKilled");
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}
