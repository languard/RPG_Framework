using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTestUnity : MonoBehaviour {

    public Entity testchar;
    public string jsonString;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateEntity()
    {
        testchar = new Entity();
        //testchar.testSkillNames = new string[] { "test 1", "test 2", "test 3" };
    }

    public void SaveToJSON()
    {
        jsonString = JsonUtility.ToJson(testchar, true);
    }

    public void LoadFromJSON()
    {
        testchar = JsonUtility.FromJson<Entity>(jsonString);
    }

    public void LevelUp()
    {
        testchar.strengthBase += 1;
    }

}
