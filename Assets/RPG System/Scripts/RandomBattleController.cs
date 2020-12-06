using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBattleController : MonoBehaviour {

    public int baseStepsToNextBattle = 10;
    public int randomBattleStepError = 5;

    public int stepsToNextBattle = 0;

    public string[] battleScenes;

    public void RefreshStepsToNextBattle()
    {
        stepsToNextBattle = baseStepsToNextBattle + Random.Range(-randomBattleStepError, randomBattleStepError + 1);
    }

    private CharController_RPG_Framework walkingCharacter;
    private Vector2Int lastKnownPlayerPosition;

	// Use this for initialization
	void Start () {

        RefreshStepsToNextBattle();

        if (battleScenes.Length == 0)
        {
            Debug.LogWarning("This scene contains no battle scenes. If it is not supposed to have battles, the RandomBattleController component should be disabled or removed. If it should have random battles, you must add at least one battle scene.");
            this.enabled = false;
            return;
        }

        walkingCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController_RPG_Framework>();
        lastKnownPlayerPosition = walkingCharacter.gridPosition;
    }
	
	// Update is called once per frame
	void Update () {
		
        if (stepsToNextBattle > 0 && walkingCharacter.gridPosition != lastKnownPlayerPosition)
        {
            stepsToNextBattle--;
            lastKnownPlayerPosition = walkingCharacter.gridPosition;
        }

        if (stepsToNextBattle <= 0 && walkingCharacter.isOnGrid)
        {
            // Transition to a random battle
            UnityEngine.SceneManagement.SceneManager.LoadScene(battleScenes[Random.Range(0, battleScenes.Length)]);
        }

	}
}
