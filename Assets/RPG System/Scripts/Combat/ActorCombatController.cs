﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorCombatController : MonoBehaviour
{

    public enum CombatState
    {
        FillingActionMeter,
        AwaitingCommand,
        AwaitingCommandExecution,
        ExecutingCommand
    }

    public CombatState combatState { get; private set; }

    private BattleController battleController;

    public float actionDelay = StaticData.DEFAULT_BATTLE_TIME;      //Normally set in editor
    private float actionTimer = 0.0f;
    public bool useReaction = false;

    public float Readiness { get { return actionTimer / actionDelay; } }

    
    //public Actor actor;
    public Entity entity;

    // Use this for initialization
    void Start()
    {
        //entity = GetComponent<Entity>();
        combatState = CombatState.FillingActionMeter;
        battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
        OnStart();
    }

    public void ResetActor()
    {
        actionTimer = 0.0f;
        combatState = CombatState.FillingActionMeter;
    }

    protected abstract void OnStart();

    // Update is called once per frame
    void Update()
    {
        if (!entity.isDisabled)
        {
            switch (combatState)
            {
                case CombatState.FillingActionMeter:
                    if(useReaction) actionTimer += Time.deltaTime * entity.reaction;
                    else actionTimer += Time.deltaTime;

                    if (actionTimer >= actionDelay)
                    {
                        combatState = CombatState.AwaitingCommand;
                        OnReadyForCommand();
                    }
                    break;
            }
        }

        if (entity.isDisabled)
        {
            combatState = CombatState.FillingActionMeter;
            actionTimer = 0;
        }

        OnUpdate();
    }

    protected abstract void OnUpdate();

    protected abstract void OnReadyForCommand();

    public abstract void ProcessCommand(CommandBase command);

    public void RegisterCommand(CommandBase command)
    {
        battleController.AddCommand(command);
        combatState = CombatState.AwaitingCommandExecution;
    }

    public List<ActorCombatController> Foes()
    {
        List<ActorCombatController> foes = new List<ActorCombatController>();
        if (this as PlayerCombatController != null)
        {
            // I'm a player, get all enemies
            foreach (ActorCombatController foe in battleController.enemyActors) if (!foe.entity.isDead) foes.Add(foe);
        }

        if (this as EnemyCombatController != null)
        {
            // I'm an enemy, get all players
            foreach (ActorCombatController foe in battleController.playerActors) if (!foe.entity.isDead) foes.Add(foe);
        }

        return foes;
    }

    public List<ActorCombatController> Allies()
    {
        List<ActorCombatController> allies = new List<ActorCombatController>();
        if (this as PlayerCombatController != null)
        {
            // I'm a player, get all players
            foreach (ActorCombatController ally in battleController.playerActors) if (!ally.entity.isDead) allies.Add(ally);
        }

        if (this as EnemyCombatController != null)
        {
            // I'm an enemy, get all enemies
            foreach (ActorCombatController ally in battleController.enemyActors) if (!ally.entity.isDead) allies.Add(ally);
        }

        return allies;
    }

}