using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkills : MonoBehaviour {

    public string[] skillNames;
    private SkillDescriptor[] skills;

	// Use this for initialization
	void Start () {
        if (!SkillDatabase.isLoaded) SkillDatabase.LoadSkills();
        if (skillNames == null) skillNames = new string[0];
        skills = new SkillDescriptor[skillNames.Length];
        for (int i = 0; i < skillNames.Length; i++)
        {
            skills[i] = SkillDatabase.GetSkill(skillNames[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Dictionary<string, SkillDescriptor> GetPlayerSkills()
    {
        Dictionary<string, SkillDescriptor> playerSkills = new Dictionary<string, SkillDescriptor>();
        foreach (SkillDescriptor skill in skills)
        {
            playerSkills.Add(skill.displayText, skill);
        }
        return playerSkills;
    }
}
