using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATBMeter : MonoBehaviour {

    public Material fillingMaterial;
    public Material fullMaterial;

    private PlayerCombatController playerCombatController;
    private Transform meterQuad;

	// Use this for initialization
	void Start () {
        playerCombatController = transform.parent.GetComponent<PlayerCombatController>();
        if (playerCombatController == null)
        {
            throw new System.Exception("ATB Meter on object [" + transform.name + "] is not the child of an object with a PlayerCombatController.");
        }

        meterQuad = transform.Find("ATB Meter");
	}
	
	// Update is called once per frame
	void Update () {
        float readiness = playerCombatController.Readiness;
        if (readiness >= 1.0f)
        {
            readiness = 1.0f;
        }
        if (readiness == 1.0f)
        {
            meterQuad.GetComponent<MeshRenderer>().material = fullMaterial;
        }
        else
        {
            meterQuad.GetComponent<MeshRenderer>().material = fillingMaterial;
        }

        this.transform.localScale = new Vector3(readiness, 1, 1);
	}
}
