using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fungus.Flowchart))]
public class Party : MonoBehaviour {

    public Fungus.Flowchart partyInvetory;
    public Fungus.Flowchart questFlags;

    public List<Entity> partyList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetPartyStats()
    {
        for(int i=0; i<partyList.Count; i++)
        {
            partyList[i].ResetStatsToBase();
            partyList[i].CalculateAllStats();
        }
    }

    public Entity GetPartMember(int index)
    {
        return partyList[index];
    }

    public Entity GetPartyMember(string name)
    {
        for(int i=0; i<partyList.Count; i++)
        {
            if (partyList[i].name == name) return partyList[i];            
        }

        return new Entity();
    }


}
