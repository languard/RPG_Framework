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
    [SerializeField, Tooltip("D1000 used, so each point is 1/10th of a %")]
    int oddsIncrease = 1;
    [SerializeField, Tooltip("D1000 used, so each point is 1/10th of a %")]
    int oddsStarting = 1;

    public bool fightEveryStep = false;
    public bool fightNever = false;

    int stepsCurrent = 0;
    int oddsCurrent = 0;

    bool startMusic = true;

	// Use this for initialization
	void Start () {
        //reload Core if GameMaster is not found.  Can't do anything without it anyways.
        GameObject gm = GameObject.Find("GameMaster");
#if UNITY_EDITOR
        PlayerPrefs.SetString("editorMap", this.gameObject.scene.name);
#endif
        if (gm == null) UnityEngine.SceneManagement.SceneManager.LoadScene("Core");

        StartMusic();

        //init encounter rate
        oddsCurrent = oddsStarting;       
    }

    //Called by the character controller after a step has been finished
    //should be used to handle any step logic, by default this is only battles
    public void FinishStep()
    {
        //start music if needed.  Normally only needed after a fight.
        if (startMusic) StartMusic();

        if (fightNever) return;

        stepsCurrent += 1;
        if (stepsCurrent <= stepsMinBetweenFight) return;

        if( ((stepsCurrent - stepsMinBetweenFight) % stepsToIncreaseOdds) == 0)
        {
            oddsCurrent += oddsIncrease;
        }

        //using a range of 1 - 1000, making each 'odds point' worth 1/10th of a percent
        if(Random.Range(1,1001) <= oddsCurrent)
        {
            //Fight!
            stepsCurrent = 0;
            oddsCurrent = oddsStarting;
            SelectMapAndStartFight();
        }
        else if(fightEveryStep)
        {
            SelectMapAndStartFight();
        }        
    }

    public void SelectMapAndStartFight()
    {
        int index = Random.Range(0, battleScenes.Count);
        startMusic = true;
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

        //dev code: SDF forces a fight. Press SD first, then F.
        if (Input.GetKey(KeyCode.S) &&
            Input.GetKey(KeyCode.D) &&
            Input.GetKeyDown(KeyCode.F)) oddsCurrent = int.MaxValue;        
	}

    public void StartMusic()
    {
        //request the first track be played
        if (backgroundMusic != null && backgroundMusic.Count > 0 && startMusic)
        {
            GameObject.Find("GameMaster").GetComponent<GameMaster>().PlayMusic(backgroundMusic[0]);
            startMusic = false;
        }
    }

}
