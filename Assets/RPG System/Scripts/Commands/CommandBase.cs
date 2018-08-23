using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFactory
{
    private static CommandFactory instance = null;
    public static CommandFactory Instance
    {
        get
        {
            if (instance == null) instance = new CommandFactory();
            return instance;
        }
    }
    private CommandFactory()
    {
    }

    public CommandBase GetCommand(CommandBase.Command commandType, ActorCombatController actor)
    {
        // Something about this feels dirty.
        CommandBase.currentActor = actor;

        // Switch statements are the WORST.
        // But the performance hit here will be minor.
        // You'd need to work very hard to have more than 2,000 commands,
        // which would still be practically instantaneous.
        // It's just not really clean.
        //
        // Maybe we can have a startup controller that tries to get a command
        // using every possible command type programmatically, bubbling exceptions
        // up loudly, forcing them to visibility?
        //
        // Meanwhile - the switch statement antipattern.
        switch (commandType)
        {
            case CommandBase.Command.Fight:
                return new FightCommand();
            default:
                throw new System.ArgumentException("Invalid type [" + commandType.ToString() + " specified.");
        }
    }
    
}

public abstract class CommandBase {

    public static ActorCombatController currentActor;
    
    public float delayTime { get; private set; }    // How long the actor must wait for the command to execute
    public float staminaCost { get; private set; }
    public float manaCost { get; private set; }
    public string displayName { get; private set; }

    [System.Flags]
    public enum Target
    {
        NONE = 0,
        ALL_ALLIES = 1,
        SELECTED_ALLY = 2,
        SELF = 4,
        SELECTED_ENEMY = 8,
        ALL_ENEMIES = 16,
        RANDOM_ENEMY = 32
    }

    public Target[] targetOptions;

    public ActorCombatController owner;
    public List<ActorCombatController> targetActors = new List<ActorCombatController>();

    private float currentDelay = 0.0f;  // How long the command has been waiting to execute
    private int _currentTargetIndex = 0;

    public bool isRetargetable { get; private set; }

    public Target currentTargetSelection
    {
        get
        {
            return targetOptions[currentTargetIndex];
        }
    }

    public int currentTargetIndex
    {
        get
        {
            return _currentTargetIndex;
        }
        set
        {
            _currentTargetIndex = value;
            if (_currentTargetIndex < 0) _currentTargetIndex = 0;
            if (_currentTargetIndex >= targetOptions.Length) _currentTargetIndex = targetOptions.Length - 1;
        }
    }

    /// <summary>
    /// An enumeration of every possible command in the entire game
    /// </summary>
    public enum Command
    {
        Fight
    }

    public CommandBase(string displayName, ActorCombatController owner, float delayTime, float staminaCost, float manaCost, Target[] targetOptions, int defaultTargetIndex, bool isRetargetable)
    {
        this.displayName = displayName;
        this.owner = owner;

        this.delayTime = delayTime;
        this.staminaCost = staminaCost;
        this.manaCost = manaCost;

        this.targetOptions = new Target[targetOptions.Length];
        for (int i = 0; i < targetOptions.Length; i++) this.targetOptions[i] = targetOptions[i];
        this.currentTargetIndex = defaultTargetIndex;

        this.isRetargetable = isRetargetable;
    }

    public void AddTarget(ActorCombatController target)
    {
        targetActors.Add(target);
    }

    public float AddTimeWaited(float time)
    {
        // The amount of time left before the command is ready to execute.
        float timeLeftToWait = delayTime - currentDelay;

        // Add the provided time. Did that put us over?
        currentDelay += time;
        if (currentDelay > delayTime)
            return timeLeftToWait;
        else
            return -1;
    }

    // Execute right now, with the chosen target and actors in their current state.
    public List<CombatEffectBase> Execute()
    {
        if (owner.actor.isDisabled) return new List<CombatEffectBase>();

        return OnExecute();

    }

    protected abstract List<CombatEffectBase> OnExecute();
}
