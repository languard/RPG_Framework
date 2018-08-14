using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    Scene currentMap;
    CharController_RPG_Framework playerController;

    bool switchingMaps = false;
    bool oldMapUnloaded = false;
    bool newMapLoaded = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(switchingMaps)
        {
            if(oldMapUnloaded && newMapLoaded)
            {
                //all done, so player can move
                SceneManager.SetActiveScene(currentMap);
                playerController.canAct = true;
                switchingMaps = false;
            }
        }
		
	}

    public void GivePartyMoney(int amount)
    {
        print("Not working yet!");
    }

    public void RegisterPlayerController(CharController_RPG_Framework curController)
    {
        //this simply overrides the previous registered controller
        playerController = curController;
    }

    public void ChangeMap(Scene targetScene, int targetX, int targetY)
    {
        switchingMaps = true;
        newMapLoaded = false;
        oldMapUnloaded = false;

        //disable player's ability to act
        playerController.canAct = false;
        //save current map data
        //unload current map scene 
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentMap);
        unloadOp.completed += MapUnloaded;
        currentMap = targetScene;
        AsyncOperation loadOP = SceneManager.LoadSceneAsync(targetScene.buildIndex, LoadSceneMode.Additive);
        loadOP.allowSceneActivation = false;
        loadOP.completed += MapLoaded;
    }

    private void MapLoaded(AsyncOperation obj)
    {
        newMapLoaded = true;
    }

    private void MapUnloaded(AsyncOperation obj)
    {
        oldMapUnloaded = true;
    }
}
