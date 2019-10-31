using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

    public AudioClip mainMenuMusic;
    public AudioSource audioSource;

    public void StartGame()
    {
        Animator coreStates = GameObject.Find("GameMaster").GetComponent<Animator>();
        coreStates.SetTrigger("StartNewGame");

       if(mainMenuMusic != null)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
