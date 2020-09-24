using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyCombatController))]
public class EnemyCombatControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Please use the Monster Editor Window to modify stats");
        EditorGUILayout.LabelField("You must also open the prefeb editor");
        if (GUILayout.Button("Open Window"))
        {
            EntityEditor.MenuCreateWindow();
        }
        //The string labels can be changed without issue
        Entity temp = (Entity)serializedObject.FindProperty("entity").objectReferenceValue;
        EditorGUILayout.LabelField("Health: " + temp.hitPoints);
        EditorGUILayout.LabelField("Endurance: " + temp.endurance);
        EditorGUILayout.LabelField("Magic: " + temp.mana);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Strength: " + temp.strength);
        EditorGUILayout.LabelField("Constitution: " + temp.constitution);
        EditorGUILayout.LabelField("Intelligence: " + temp.intelligence);
        EditorGUILayout.LabelField("Willpower: " + temp.willpower);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Physical Armor: " + temp.physicalArmor);
        EditorGUILayout.LabelField("Magic armor: " + temp.magicArmor);

    }
}