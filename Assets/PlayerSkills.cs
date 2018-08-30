using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkills : MonoBehaviour {

    public SkillDescriptor[] skills;

	// Use this for initialization
	void Start () {
		
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
