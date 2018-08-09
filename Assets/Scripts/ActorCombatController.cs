using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorCombatController : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        combatState = CombatState.FillingActionMeter;
        battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
    }

    // Update is called once per frame
    void Update () {
		
        switch (combatState)
        {
            case CombatState.FillingActionMeter:
                actionTimer += Time.deltaTime;
                if (actionTimer >= actionDelay)
                {
                    OnReadyForCommand();
                }
                break;
        }

	}

    protected abstract void OnReadyForCommand();

    public abstract void ProcessCommand(Command)
}
