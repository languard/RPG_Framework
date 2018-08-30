using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemySkills : MonoBehaviour {

    public SkillDescriptor[] skills;
    public int[] weights;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public SkillDescriptor ChooseSkill()
    {
        int runningTotal = 0;
        List<int> thresholds = new List<int>();
        List<SkillDescriptor> candidateSkills = new List<SkillDescriptor>();

        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].manaCost <= GetComponent<Actor>().mana)
            {
                thresholds.Add(weights[i]);
                candidateSkills.Add(skills[i]);
                runningTotal += weights[i];
            }

        }

        if (candidateSkills.Count == 0) return null;

        int diceRoll = Random.Range(0, runningTotal);
        for (int i = 1; i < thresholds.Count; i++)
        {
            if (thresholds[i] >= diceRoll) return candidateSkills[i - 1];
        }
        return candidateSkills[candidateSkills.Count - 1];
    }
}
