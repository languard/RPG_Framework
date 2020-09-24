using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCombatController))]
public class PlayerCombatControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Please use the Player Editor Window");
        if (GUILayout.Button("Open Window (not implemented)"))
        {
            //EntityEditor.MenuCreateWindow();
        }
    }
}


