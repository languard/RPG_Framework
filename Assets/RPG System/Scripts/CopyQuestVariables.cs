using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class CopyQuestVariables : MonoBehaviour {

    public string[] varNames;

    GameMaster gm;

	// Use this for initialization
	void Start () {

        gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        Flowchart gmQuest = gm.GetQuestFlowchart();
        Flowchart target = this.GetComponent<Flowchart>();

        Variable sourceVar = null;        

        for(int i=0; i<varNames.Length; i++)
        {
            sourceVar = gmQuest.GetVariable(varNames[i]);

            StringVariable tempstr = sourceVar as StringVariable;
            if(tempstr != null)
            {
                target.SetStringVariable(varNames[i], tempstr.Value);
                continue;
            }

            BooleanVariable tempBool = sourceVar as BooleanVariable;
            if (tempBool != null)
            {
                target.SetBooleanVariable(varNames[i], tempBool.Value);
                continue;
            }

            IntegerVariable tempInt = sourceVar as IntegerVariable;
            if (tempInt != null)
            {
                target.SetIntegerVariable(varNames[i], tempInt.Value);
                continue;
            }

            FloatVariable tempFloat = sourceVar as FloatVariable;
            if (tempFloat != null)
            {
                target.SetFloatVariable(varNames[i], tempFloat.Value);
                continue;
            }
        }
		
	}

    public void CopyFlowchartToGameMaster()
    {
        Flowchart gmQuest = gm.GetQuestFlowchart();
        Flowchart target = this.GetComponent<Flowchart>();

        Variable sourceVar = null;

        for (int i = 0; i < varNames.Length; i++)
        {
            sourceVar = target.GetVariable(varNames[i]);

            StringVariable tempstr = sourceVar as StringVariable;
            if (tempstr != null)
            {
                gmQuest.SetStringVariable(varNames[i], tempstr.Value);
                continue;
            }

            BooleanVariable tempBool = sourceVar as BooleanVariable;
            if (tempBool != null)
            {
                gmQuest.SetBooleanVariable(varNames[i], tempBool.Value);
                continue;
            }

            IntegerVariable tempInt = sourceVar as IntegerVariable;
            if (tempInt != null)
            {
                gmQuest.SetIntegerVariable(varNames[i], tempInt.Value);
                continue;
            }

            FloatVariable tempFloat = sourceVar as FloatVariable;
            if (tempFloat != null)
            {
                gmQuest.SetFloatVariable(varNames[i], tempFloat.Value);
                continue;
            }
        }
    }
}
