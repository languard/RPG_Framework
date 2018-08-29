using System.Collections;
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

    public float actionDelay = 3.0f;
    private float actionTimer = 0.0f;

    public float Readiness { get { return actionTimer / actionDelay; } }

    public Dictionary<CommandBase.Command, string> commandNames = new Dictionary<CommandBase.Command, string>();

    [HideInInspector]
    public Actor actor;

    // Use this for initialization
    void Start()
    {
        actor = GetComponent<Actor>();
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
        if (!actor.isDisabled)
        {
            switch (combatState)
            {
                case CombatState.FillingActionMeter:
                    actionTimer += Time.deltaTime;
                    if (actionTimer >= actionDelay)
                    {
                        combatState = CombatState.AwaitingCommand;
                        OnReadyForCommand();
                    }
                    break;
            }
        }

        if (actor.isDisabled)
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
            foreach (ActorCombatController foe in battleController.enemyActors) if (!foe.actor.isDead) foes.Add(foe);
        }

        if (this as EnemyCombatController != null)
        {
            // I'm an enemy, get all players
            foreach (ActorCombatController foe in battleController.playerActors) if (!foe.actor.isDead) foes.Add(foe);
        }

        return foes;
    }

    public List<ActorCombatController> Allies()
    {
        List<ActorCombatController> allies = new List<ActorCombatController>();
        if (this as PlayerCombatController != null)
        {
            // I'm a player, get all players
            foreach (ActorCombatController ally in battleController.playerActors) if (!ally.actor.isDead) allies.Add(ally);
        }

        if (this as EnemyCombatController != null)
        {
            // I'm an enemy, get all enemies
            foreach (ActorCombatController ally in battleController.enemyActors) if (!ally.actor.isDead) allies.Add(ally);
        }

        return allies;
    }

}