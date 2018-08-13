using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGMToFungus : MonoBehaviour {

    public Demo demo;

    [SerializeField]GameMaster GM;

	// Use this for initialization
	void Start () {

        if (GM == null) GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        //Fungus.Flowchart fc = GetComponent<Fungus.Flowchart>();
        //Fungus.ObjectVariable ov = null;
        //ov = fc.GetVariable("GameMaster") as Fungus.ObjectVariable;
        //ov.Value = GameObject.Find("GameMaster");
    }

    public void GivePartyGold(int amount)
    {
        GM.GivePartyMoney(amount);
    }


	
	
}
