using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : ActorCombatController
{



    private Animator anim; //The parent animator.

    /// <summary>
    /// The time in seconds between character actions.
    /// </summary>
    public float actionDelay = 3.0f;
    private float actionTimer = 0.0f;

    public float fightDelay = 0.5f;
    private float fightTimer = 0.0f;

    public float fightExecutionTime = 0.25f;
    private float fightExecutionTimer = 0.0f;

    private BattleController battleController;

    public bool isActiveAwaitingCommand { get; private set; }
   
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        activeAwaitingCommand = false;
    }
	
	// Update is called once per frame
	void Update () {
        anim.SetFloat("moveX", -1f);
        anim.SetBool("isMoving", false);
        anim.SetBool("isRunning", false);

        if (isActiveAwaitingCommand)
        {
            // For now, short-circuit all of it and just have it fight the first enemy
            FightCommand fightCommand = new FightCommand();
            fightCommand.AddTarget(battleController.enemyActors[0]);
            battleController.AddCommand(fightCommand);
        }

        switch (combatState)
        {
            case CombatState.FillingActionMeter:
                actionTimer += Time.deltaTime;
                if (actionTimer >= actionDelay)
                {
                    print("Action meter filled");
                    battleController.PlayerUnitReadyToSelectCommand(this);
                }
                break;
            case CombatState.ActiveAwaitingCommand:
                // For now we're only doing one thing; eventually we'll use this to inject player control.
                combatState = CombatState.AwaitingCommandExecution;
                fightTimer = 0.0f;
                print("Fight command selected automatically");
                break;
            case CombatState.AwaitingCommandExecution:
                // While we're fight-only, we'll await filling up of the fightDelay meter.
                fightTimer += Time.deltaTime;
                if (fightTimer >= fightDelay)
                {
                    // TODO: Add my Fight action to the battle controller action queue.
                    combatState = CombatState.AwaitingTurn;
                    print("Delay complete, awaiting turn");
                }
                break;
            case CombatState.AwaitingTurn:
                // TODO: Check in with the battle controller to see if it's my turn yet.
                // Alternately, provide a Turn callback to the controller.
                // Alternatively, implement this as an interface and create a method on the controller to say "okay do your thing."
                // Alternatively, implement this as an abstract base class and inject player or monster methods as appropriate.
                // Alternatively, implement this as an absract base class and then inherit a Player and Enemy abstract base class from that,
                // since one will rely on user input and the other will rely on a decision making AI.
                fightExecutionTimer = 0.0f;
                print("It's my turn (" + gameObject.name + "), executing");
                combatState = CombatState.Executing;
                break;
            case CombatState.Executing:
                fightExecutionTimer += Time.deltaTime;
                if (fightExecutionTimer >= fightExecutionTime)
                {
                    // Process the effect of the current action
                    combatState = CombatState.FillingActionMeter;
                    actionTimer = 0.0f;
                    print("Action was executed");
                }
                break;
        }

	}

    protected override void OnReadyForCommand()
    {
        battleController.PlayerUnitReadyToSelectCommand(this);
    }

    public void ActivateForCommand()
    {
        isActiveAwaitingCommand = true;
    }
}
