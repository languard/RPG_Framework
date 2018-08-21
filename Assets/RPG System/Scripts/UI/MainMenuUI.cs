using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {


    public void StartGame()
    {
        Animator coreStates = GameObject.Find("GameMaster").GetComponent<Animator>();
        coreStates.SetTrigger("StartNewGame");
    }
}
