using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour {

    bool startTransition = false;

    [SerializeField]
    UnityEngine.SceneManagement.Scene targetScene;

    [SerializeField]
    int targetX;
    [SerializeField]
    int targetY;


    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && Time.timeSinceLevelLoad > 0.25f)
        {
            CharController_RPG_Framework controller = other.gameObject.GetComponent<CharController_RPG_Framework>();
            if (controller.isOnGrid) startTransition = true;
        }
    }

    public void LateUpdate()
    {
        if(startTransition)
        {
            GameMaster GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
            GM.ChangeMap(targetScene, targetX, targetY);
        }
    }

}
