using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGMToFungus : MonoBehaviour {

    public Demo demo;

    [SerializeField]GameMaster GM;

	// Use this for initialization
	void Start () {

        if (GM == null) GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        Fungus.Flowchart fc = GetComponent<Fungus.Flowchart>();
        Fungus.GameObjectVariable fGO = null;
        fGO = fc.GetVariable("Self") as Fungus.GameObjectVariable;
        fGO.Value = this.gameObject;
    }

    public void GivePartyGold(int amount)
    {
        GM.GivePartyMoney(amount);
    }


	
	
}
