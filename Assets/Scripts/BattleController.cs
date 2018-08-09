using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    private Queue<PlayerCombatController> playerUnitsReadyToSelectCommand = new Queue<PlayerCombatController>();


    public List<ActorCombatController> enemyActors = new List<ActorCombatController>();
    public List<ActorCombatController> playerActors = new List<ActorCombatController>();

    public List<CommandBase> commands = new List<CommandBase>();

    private Queue<CommandBase> activeCommands = new Queue<CommandBase>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (playerUnitsReadyToSelectCommand.Count > 0)
        {
            if (!playerUnitsReadyToSelectCommand.Peek().isActiveAwaitingCommand)
            {
                // Activate the head, but don't dequeue it until it either
                // passes or enters a command.
                playerUnitsReadyToSelectCommand.Peek().ActivateForCommand();
            }
        }

        Dictionary<float, CommandBase> completedCommands = new Dictionary<float, CommandBase>();
        for (int i = 0; i < commands.Count; i++)
        {
            float waitReturn = commands[i].AddTimeWaited(Time.deltaTime);
            if (waitReturn >= 0)
            {
                while (completedCommands.ContainsKey(waitReturn)) waitReturn += 0.001f;
                completedCommands.Add(waitReturn, commands[i]);
            }
        }

        List<float> waitTimes = new List<float>();
        foreach (float waitTime in completedCommands.Keys) waitTimes.Add(waitTime);
        waitTimes.Sort();

        foreach (float waitTime in waitTimes)
        {
            commands.Remove(completedCommands[waitTime]);
            activeCommands.Enqueue(completedCommands[waitTime]);
        }

        if (activeCommands.Count > 0)
        {
            // Process the effects... hm... within the command itself?
            // Then hand the results off to the... hrng...
            // Rules are rules. We're wasting time at this point.
        }

	}

    public void PlayerUnitReadyToSelectCommand(PlayerCombatController playerUnit)
    {
        playerUnitsReadyToSelectCommand.Enqueue(playerUnit);
    }

    public void AddCommand(CommandBase command)
    {
        commands.Add(command);
    }
}
