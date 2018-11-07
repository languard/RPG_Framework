using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void _NewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Core");
    }

    public void _QuitGame()
    {
        Application.Quit();
    }
}
