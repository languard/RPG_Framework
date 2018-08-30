using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLogic : MonoBehaviour {

    [SerializeField]
    List<AudioClip> backgroundMusic;

	// Use this for initialization
	void Start () {
        //request the first track be played
        if(backgroundMusic != null && backgroundMusic.Count > 0)
        {
            GameObject.Find("GameMaster").GetComponent<GameMaster>().PlayMusic(backgroundMusic[0]);
        }
	}

    
	
	// Update is called once per frame
	void Update () {
		


	}

    public void StartMusic()
    {

    }

}
