using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandBase {

    public float delayTime { get; private set; }
    public float staminaCost { get; private set; }
    public float manaCost { get; private set; }

    public const int ALL_ALLIES = 3;
    public const int SELECTED_ALLY = 2;
    public const int SELF = 1;
    public const int SELECTED_ENEMY = 0;
    public const int ALL_ENEMIES = -1;
    public const int RANDOM_ENEMY = -2;

    public int[] targetOptions;
    public int defaultTargetIndex = 0;

    public ActorCombatController owner;
    public List<ActorCombatController> targetActors = new List<ActorCombatController>();

    private float currentDelay = 0.0f;

    public CommandBase(ActorCombatController owner, float delayTime, float staminaCost, float manaCost, int[] targetOptions, int defaultTargetIndex)
    {
        this.owner = owner;

        this.delayTime = delayTime;
        this.staminaCost = staminaCost;
        this.manaCost = manaCost;

        this.targetOptions = new int[targetOptions.Length];
        for (int i = 0; i < targetOptions.Length; i++) this.targetOptions[i] = targetOptions[i];
        this.defaultTargetIndex = defaultTargetIndex;
    }

    public void AddTarget(ActorCombatController target)
    {
        targetActors.Add(target);
    }

    public float AddTimeWaited(float time)
    {
        float timeLeftToWait = delayTime - currentDelay;
        currentDelay += time;
        if (currentDelay > delayTime)
            return timeLeftToWait;
        else
            return -1;
    }
}
