using Assets.RPG_System.Scripts.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : ActorCombatController
{
    private EnemySkills skills;

    protected override void OnStart()
    {
        skills = GetComponent<EnemySkills>();
    }

    protected override void OnUpdate()
    {
        GetComponent<SpriteRenderer>().enabled = !actor.isDisabled;
    }

    protected override void OnReadyForCommand()
    {
        // Choose a skill to use.
        SkillDescriptor skill = skills.ChooseSkill();
        // Based on this we'll create a command.

        if (skill == null)
        {
            print("Actor " + transform.name + " has no available skills among the " + skills.weightedSkills.Length + " to choose from");
            ResetActor();
            return;
        }

        // Might be overkill to do all this since we're basing
        // actions on startTarget only.
        // But, we may eventually have a Charm/Confusion status.
        CommandBase.Target skillTargets = skill.targets;
        List<CommandBase.Target> targets = new List<CommandBase.Target>();
        foreach (CommandBase.Target target in (CommandBase.Target[])System.Enum.GetValues(typeof(CommandBase.Target)))
        {
            if ((skillTargets & target) > 0) targets.Add(target);
        }
        int defaultTargetIndex = targets.IndexOf(skill.startTarget);
        if (defaultTargetIndex < 0) defaultTargetIndex = 0;

        LoadedCommand loadedCommand = new LoadedCommand(skill.displayText, this, skill.delay, 0, skill.manaCost, targets.ToArray(), defaultTargetIndex, skill.isRetargetable, skill.effects);

        // Select target(s) based on default behavior for now
        switch (targets[defaultTargetIndex])
        {
            case CommandBase.Target.ALL_ALLIES:
                foreach (ActorCombatController ally in Allies()) loadedCommand.AddTarget(ally);
                break;
            case CommandBase.Target.ALL_ENEMIES:
                foreach (ActorCombatController foe in Foes()) loadedCommand.AddTarget(foe);
                break;
            case CommandBase.Target.SELECTED_ALLY:
                loadedCommand.AddTarget(RandomTarget(Allies()));
                break;
            case CommandBase.Target.SELECTED_ENEMY:
                loadedCommand.AddTarget(RandomTarget(Foes()));
                break;
            case CommandBase.Target.RANDOM_ENEMY:
                loadedCommand.AddTarget(RandomTarget(Foes()));
                break;
            case CommandBase.Target.SELF:
                loadedCommand.AddTarget(this);
                break;
        }

        RegisterCommand(loadedCommand);

    }

    private ActorCombatController RandomTarget(List<ActorCombatController> targets)
    {
        return targets[Random.Range(0, targets.Count)];
    }
    
    public override void ProcessCommand(CommandBase command)
    {
        ResetActor();
    }
}
