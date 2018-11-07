using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDamage : MonoBehaviour {

    public string damage;
    public GUIStyle displayStyle;
    public float lifetime = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(this);
        }
	}

    private void OnGUI()
    {
        Vector3 screenLocation = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screenLocation.x - 30, Screen.height - (screenLocation.y - 10), 60, 20), damage, displayStyle);
    }
}
