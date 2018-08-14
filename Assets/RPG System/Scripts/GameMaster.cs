using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    [SerializeField] string firstScene = "NOT SET";
    [SerializeField, Tooltip("Leave blank if no override needed")] string editorOverride;

    Scene currentMap;
    CharController_RPG_Framework playerController;

    bool switchingMaps = false;
    bool oldMapUnloaded = false;
    bool newMapLoaded = false;

    WaitForSeconds loadDelay;

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        currentMap = SceneManager.GetSceneByName(editorOverride);
#endif

        loadDelay = new WaitForSeconds(0.1f);
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

    public void ChangeMap(string targetScene, int targetX, int targetY)
    {
        StartCoroutine(HandleChangeMap(targetScene, targetX, targetY));
    }

    IEnumerator HandleChangeMap(string targetScene, int targetX, int targetY)
    {
        switchingMaps = true;
        newMapLoaded = false;
        oldMapUnloaded = false;

        print("Current scene is:" + currentMap.name);

        //disable player's ability to act
        playerController.canAct = false;
        //save current map data
        //unload current map scene 
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentMap);
        unloadOp.completed += MapUnloaded;

        while (!oldMapUnloaded) yield return loadDelay;
        print("old map unloaded");

        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
        currentMap = SceneManager.GetSceneByName(targetScene);        
        yield return loadDelay; //giving scene time to load
        SceneManager.SetActiveScene(currentMap);
        newMapLoaded = true;

        print("Setting player location");
        playerController.SetLocation(targetX, targetY);
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
