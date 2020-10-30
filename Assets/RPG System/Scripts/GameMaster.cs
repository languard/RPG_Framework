using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public static GameMaster instance;

    [SerializeField] string startingMap = "NOT SET";
    [SerializeField] int startX = 0;
    [SerializeField] int startY = 0;
    [SerializeField, Tooltip("Leave blank if no override needed")] string editorOverride;
    [SerializeField, Tooltip("Automatically load the Map you were working on.")] bool loadPriorScene = true;

    Scene currentMap;
    Scene returnScene;

    CharController_RPG_Framework playerController;

    bool switchingMaps = false;
    bool oldMapUnloaded = false;
    bool newMapLoaded = false;
    bool inBattle = false;

    public bool showSystemChat = false;
    bool systemChatUp = false;

    WaitForSeconds loadDelay;

    Party party;

    public AudioSource music;
    public AudioSource sfx;
    public AudioSource voice;

    Fungus.Flowchart questFlags;
    Fungus.Flowchart inventory;
    Fungus.Flowchart systemChat;

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        currentMap = SceneManager.GetSceneByName(editorOverride);
#endif

        loadDelay = new WaitForSeconds(0.1f);
        party = GetComponentInChildren<Party>();

        questFlags = GameObject.Find("QuestFlags").GetComponent<Fungus.Flowchart>();
        inventory = GameObject.Find("Inventory").GetComponent<Fungus.Flowchart>();
        systemChat = GameObject.Find("SystemChat").GetComponent<Fungus.Flowchart>();

        if (SceneManager.sceneCount > 1) SceneManager.LoadScene("Core");

        if (instance == null) instance = this;
        else Debug.LogError("WARNING: multiple instances of GameMaster created!");
    }
	
	// Update is called once per frame
	void Update () {

        if (showSystemChat && !switchingMaps)
        {
            showSystemChat = false;
            systemChatUp = true;
            systemChat.ExecuteBlock("SystemChat");
            playerController.canAct = false;
        }

        if (switchingMaps)
        {
            if(oldMapUnloaded && newMapLoaded)
            {
                //all done, so player can move
                SceneManager.SetActiveScene(currentMap);
                playerController.canAct = true;
                switchingMaps = false;
            }
        }        
		
        if(systemChatUp)
        {
            if(!systemChat.FindBlock("SystemChat").IsExecuting())
            {
                systemChatUp = false;
                playerController.canAct = true;
            }
        }
	}

    public void DisableCharacterMovement()
    {
        playerController.canMove = false;
        playerController.canAct = false;
    }

    public void EnableCharacterMovement()
    {
        //do not enable character movement if we are in battle mode
        if (!inBattle)
        {
            playerController.canMove = true;
            playerController.canAct = true;
        }
    }

    public void LoadStartMap()
    {
        //this is different than the normal ChangeMap because there is no map to unload
        oldMapUnloaded = true;
        newMapLoaded = false;

        StartCoroutine(HandleLoadStartingMap());
    }

    IEnumerator HandleLoadStartingMap()
    {
        if (editorOverride != "") startingMap = editorOverride;

#if UNITY_EDITOR
        if (loadPriorScene)
        {
            string priorScene = PlayerPrefs.GetString("editorMap", "");
            if (priorScene != "") startingMap = priorScene;
            print(priorScene);
        }
#endif

        SceneManager.LoadScene(startingMap, LoadSceneMode.Additive);
        currentMap = SceneManager.GetSceneByName(startingMap);
        yield return loadDelay; //giving scene time to load
        SceneManager.SetActiveScene(currentMap);

        //starting game, so make sure player stats are calculated correctly
        party.ResetPartyStats();
        
        //print("Setting player location");
        playerController.SetLocation(startX, startY);
        newMapLoaded = true;
    }

    public void GivePartyMoney(int amount)
    {
        party.partyGold += amount;
    }

    public void TakePartyMoney(int amount)
    {
        party.partyGold -= amount;
        if (party.partyGold < 0) party.partyGold = 0;
    }

    public int GetPartyMoney()
    {
        return party.partyGold;
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

    public void LoadBattleScene(string sceneName)
    {
        inBattle = true;
        playerController.DisableForBattle();
        returnScene = currentMap;
        StartCoroutine(HandleBattleMapLoad(sceneName));
    }

    public void BattleDone(int gold)
    {
        inBattle = false;
        party.partyGold += gold;
        Fungus.IntegerVariable iv = systemChat.GetVariable<Fungus.IntegerVariable>("gold");
        iv.Value = gold;
        showSystemChat = true;

        StartCoroutine(HandleBattleDone());
    }

    IEnumerator HandleBattleDone()
    {
        oldMapUnloaded = false;
        AsyncOperation mapUnload = SceneManager.UnloadSceneAsync(currentMap);
        mapUnload.completed += MapUnloaded;
        while (!oldMapUnloaded) yield return loadDelay;
        SceneManager.SetActiveScene(returnScene);
        currentMap = returnScene;
        playerController.ActivateController();
    }

    IEnumerator HandleBattleMapLoad(string targetScene)
    {
        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
        yield return loadDelay;        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
        currentMap = SceneManager.GetActiveScene();
    }

    IEnumerator HandleChangeMap(string targetScene, int targetX, int targetY)
    {
        switchingMaps = true;
        newMapLoaded = false;
        oldMapUnloaded = false;

        //print("Current scene is:" + currentMap.name);

        //disable player's ability to act
        playerController.canAct = false;
        //save current map data
        //unload current map scene 
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentMap);
        unloadOp.completed += MapUnloaded;

        while (!oldMapUnloaded) yield return loadDelay;
        //print("old map unloaded");

        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
        currentMap = SceneManager.GetSceneByName(targetScene);        
        yield return loadDelay; //giving scene time to load
        SceneManager.SetActiveScene(currentMap);
        newMapLoaded = true;

        //print("Setting player location");
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

    public void StartNewGame()
    {
        party.ResetPartyStats();
    }

    public void PlayMusic(AudioClip clip, bool repeat = true)
    {
        music.clip = clip;
        music.loop = repeat;
        music.Stop();
        music.Play();

    }

    public Entity GetPartyMember(string name)
    {
        return party.GetPartyMember(name);
    }

    public Entity GetPartyMemberByID(string id)
    {
        return party.GetPartyMemberByID(id);
    }

    public Fungus.Flowchart GetQuestFlowchart()
    {
        return questFlags;
    }

    public Fungus.Flowchart GetInventory()
    {
        return inventory;
    }
    
}
