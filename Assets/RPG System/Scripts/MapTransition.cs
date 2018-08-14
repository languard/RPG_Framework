using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour {

    bool startTransition = false;

    [SerializeField]
    string targetScene;

    [SerializeField]
    int targetX;
    [SerializeField]
    int targetY;

    float timeCreated = 0;
    bool ignore = true;

    private void Start()
    {
        timeCreated = Time.realtimeSinceStartup;

    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(!ignore)
        {
            CharController_RPG_Framework controller = other.gameObject.GetComponent<CharController_RPG_Framework>();
            if (controller.isOnGrid) startTransition = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && (Time.realtimeSinceStartup - timeCreated) > 0.25f)
        {
            ignore = false;
        }
    }

    public void LateUpdate()
    {
        if(startTransition)
        {
            GameMaster GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
            GM.ChangeMap(targetScene, targetX, targetY);
            Destroy(this);
        }
    }

}
