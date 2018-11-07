using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public bool IsValid()
    {
        if (owner.entity.mana < manaCost) return false;
        // Add other conditions of validity here;
        // do not short-circuit this call

        return true;
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
        if (owner.entity.isDisabled) return new List<CombatEffectBase>();

        return OnExecute();

    }

    protected abstract List<CombatEffectBase> OnExecute();
}
