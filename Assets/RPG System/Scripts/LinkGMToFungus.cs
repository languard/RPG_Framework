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

    public void TakePartyGold(int amount)
    {
        GM.TakePartyMoney(amount);
    }

    public bool PartyHasGold(int amount)
    {
        if (GM.GetPartyMoney() >= amount) return true;
        else return false;
    }
	
    public void StartFight(string fightName)
    {
        GM.LoadBattleScene(fightName);
    }

    public void HealParty()
    {
        //This heals a single character
        //Additional lines needed for each character
        GM.GetPartyMember("The Witch").FullHeal();
    }

    public void IncreaseIntelligence(int amount)
    {
        GM.IncreaseIntelligence(amount);
    }

    public void IncreaseWillpower(int amount)
    {
        GM.IncreaseWillpower(amount);
    }

    public void IncreaseStrength(int amount)
    {
        GM.IncreaseStrength(amount);
    }

    public void IncreaseConsitution(int amount)
    {
        GM.IncreaseConstitution(amount);
    }

}
