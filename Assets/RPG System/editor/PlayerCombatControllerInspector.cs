using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCombatController))]
public class PlayerCombatControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //EditorGUILayout.LabelField("Please use the Party Editor Window");
        //if (GUILayout.Button("Open Window"))
        //{
        //    PartyEditorWindow.MenuCreateWindow();
        //}

        serializedObject.Update();
        SerializedProperty combatID = serializedObject.FindProperty("playerCombatID");        
        combatID.stringValue = EditorGUILayout.TextField("Combat ID: ", combatID.stringValue);
        serializedObject.ApplyModifiedProperties();
    }
}


