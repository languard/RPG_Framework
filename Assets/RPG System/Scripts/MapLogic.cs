using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLogic : MonoBehaviour {

    [SerializeField]
    List<AudioClip> backgroundMusic;

    [SerializeField]
    List<string> battleScenes;

    [SerializeField]
    int stepsMinBetweenFight = 10;
    [SerializeField]
    int stepsToIncreaseOdds = 10;
    [SerializeField]
    int oddsIncrease = 1;
    [SerializeField]
    int oddsStarting = 1;

    bool fightEveryStep = false;
    bool fightNever = false;

    int stepsCurrent = 0;
    int oddsCurrent = 0;

	// Use this for initialization
	void Start () {
        StartMusic();
	}

    //Called by the character controller after a step has been finished
    //should be used to handle any step logic, by default this is only battles
    public void FinishStep()
    {
        if (fightNever) return;

        stepsCurrent += 1;
        if (stepsCurrent <= stepsMinBetweenFight) return;

        if( ((stepsCurrent - stepsMinBetweenFight) % stepsToIncreaseOdds) == 0)
        {
            oddsCurrent += oddsIncrease;
        }

        //using a range of 1 - 1000, making each 'odds point' worth 1/10th of a percent
        if(Random.Range(1,1001) >= oddsCurrent)
        {
            //Fight!
            stepsCurrent = 0;
            oddsCurrent = 0;
            SelectMapAndStartFight();
        }
    }

    public void SelectMapAndStartFight()
    {
        int index = Random.Range(0, battleScenes.Count);
        GameObject.Find("GameMaster").GetComponent<GameMaster>().LoadBattleScene(battleScenes[index]);
    }

    public void SetAlwaysFight(bool value)
    {
        fightEveryStep = value;
        fightNever = false;
    }
    
	public void SetNeverFight(bool value)
    {
        fightNever = value;
        fightEveryStep = false;
    }

	// Update is called once per frame
	void Update () {

        print("Chatter!");

	}

    public void StartMusic()
    {
        //request the first track be played
        if (backgroundMusic != null && backgroundMusic.Count > 0)
        {
            GameObject.Find("GameMaster").GetComponent<GameMaster>().PlayMusic(backgroundMusic[0]);
        }
    }

}
