using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class WeightedSkill : ScriptableObject
{
    public static WeightedSkill Create(SkillDescriptor skill, int weight)
    {
        WeightedSkill newInstance = ScriptableObject.CreateInstance<WeightedSkill>();
        newInstance.skill = skill;
        newInstance.weight = weight;
        return newInstance;
    }

    public SkillDescriptor skill;
    public int weight;
}
