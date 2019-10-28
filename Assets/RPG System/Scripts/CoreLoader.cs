using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Scene coreScene = SceneManager.GetSceneByName("Core");
        if(!coreScene.isLoaded)
        {
            SceneManager.LoadScene("Core", LoadSceneMode.Additive);
        }
		
	}

}
