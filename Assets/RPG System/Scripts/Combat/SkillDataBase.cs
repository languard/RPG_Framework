using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class EffectDescriptor
{
    public CombatEffectBase.Effect effectType;
    public string effectExpression;
}

[System.Serializable]
public class SkillDescriptor
{
    public string name = "New Skill";
    public string displayText = "Enter Display Text";
    public float delay = 2.0f;
    public int manaCost = 0;
    public CommandBase.Target targets = CommandBase.Target.SELECTED_ENEMY | CommandBase.Target.SELECTED_ALLY;
    public CommandBase.Target startTarget = CommandBase.Target.SELECTED_ENEMY;
    public bool isRetargetable = true;
    public EffectDescriptor[] effects;

    public void SetEffects(List<EffectDescriptor> effectsList)
    {
        effects = effectsList.ToArray();
    }

    public void RemoveEffect(EffectDescriptor effect)
    {
        List<EffectDescriptor> effectList = new List<EffectDescriptor>();
        if (effects != null) effectList.AddRange(effects);

        effectList.Remove(effect);
        SetEffects(effectList);
    }

    public void AddEffect(EffectDescriptor effect)
    {
        List<EffectDescriptor> effectList = new List<EffectDescriptor>();
        if (effects != null) effectList.AddRange(effects);

        effectList.Add(effect);
        SetEffects(effectList);
    }
}

public class SkillDatabase
{
    public static string jsonFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "skills.json");
    private static Dictionary<string, SkillDescriptor> skillsByName = new Dictionary<string, SkillDescriptor>();

    public static SkillDescriptor GetSkill(string name)
    {
        if (!skillsByName.ContainsKey(name))
        {
            return skillsByName[skillsByName.Keys.First()];
        }
        else
        {
            return skillsByName[name];
        }
    }


    public static List<string> FindSkills(string likeSearch)
    {
        List<string> skillNames = new List<string>();
        foreach (string name in skillsByName.Keys)
        {
            if (likeSearch.Length == 0 || name.Contains(likeSearch)) skillNames.Add(name);
        }
        skillNames.Sort();
        return skillNames;
    }

    public static List<SkillDescriptor> GetAllSkills()
    {
        return skillsByName.Values.ToList<SkillDescriptor>();
    }

    public static void UpdateSkill(SkillDescriptor descriptor)
    {
        if (descriptor == null) throw new System.ArgumentNullException("descriptor");
        string name = descriptor.name;
        if (!skillsByName.ContainsKey(name))
            skillsByName.Add(name, descriptor);
        else
            skillsByName[name] = descriptor;
    }

    public static void RenameSkill(string oldName, SkillDescriptor currentSkill)
    {
        if (oldName == currentSkill.name)
        {
            skillsByName[oldName] = currentSkill;
            return;
        }

        if (skillsByName.ContainsKey(currentSkill.name))
            skillsByName[currentSkill.name] = currentSkill;
        else
            skillsByName.Add(currentSkill.name, currentSkill);

        skillsByName.Remove(oldName);
    }

    public static bool isLoaded
    {
        get
        {
            return skillsByName.Count > 0;
        }
    }

    public static void LoadSkills()
    {
        string[] jsonLines = System.IO.File.ReadAllLines(jsonFilePath);
        skillsByName.Clear();
        foreach (string line in jsonLines)
        {
            SkillDescriptor skillDescriptor = JsonUtility.FromJson<SkillDescriptor>(line);
            skillsByName.Add(skillDescriptor.name, skillDescriptor);
        }
    }

    public static void SaveSkills()
    {
        List<string> jsonLines = new List<string>();
        foreach (string name in skillsByName.Keys)
        {
            jsonLines.Add(JsonUtility.ToJson(skillsByName[name]));
        }
        System.IO.File.WriteAllLines(jsonFilePath, jsonLines.ToArray());
    }
}
