using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Entity))]
public class EntityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Please use the Entity Editor Window");
        if (GUILayout.Button("Open Window"))
        {
            EnemyEntityEditor.MenuCreateWindow();
        }
    }
}


