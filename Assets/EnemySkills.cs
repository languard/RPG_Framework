using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkills : MonoBehaviour {

    private List<SkillDescriptor> skills = new List<SkillDescriptor>();
    private List<int> weights = new List<int>();

    public List<SkillDescriptor> Skills { get { return skills; } }
    public List<int> Weights { get { return weights; } }

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

        for (int i = 0; i < skills.Count; i++)
        {
            thresholds.Add(weights[i]);
            skills.Add(skills[i]);
            runningTotal += weights[i];
        }

        int diceRoll = Random.Range(0, runningTotal);
        for (int i = 1; i < thresholds.Count; i++)
        {
            if (thresholds[i] >= diceRoll) return skills[i - 1];
        }
        return skills[skills.Count - 1];
    }
}
