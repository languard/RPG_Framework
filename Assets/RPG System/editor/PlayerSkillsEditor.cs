using System;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomEditor(typeof(PlayerSkills))]
public class PlayerSkillsEditor : Editor
{
    private List<string> skillNames = new List<string>();
    private int selectedSkillIndex = 0;

    private void OnEnable()
    {        
        // Reload skills database
        SkillDatabase.LoadSkills();
        skillNames = SkillDatabase.FindSkills(string.Empty);

        PlayerSkills playerSkills = target as PlayerSkills;
        List<string> invalidSkillNames = new List<string>();
        if (playerSkills.skillNames == null) playerSkills.skillNames = new string[0];
        foreach (string skillName in playerSkills.skillNames)
        {
            if (skillNames.Contains(skillName))
            {
                skillNames.Remove(skillName);
            }
            else
            {
                invalidSkillNames.Add(skillName);
            }
        }
        List<string> newSkillNames = new List<string>();
        newSkillNames.AddRange(playerSkills.skillNames);
        foreach (string invalidSkillName in invalidSkillNames)
        {
            newSkillNames.Remove(invalidSkillName);
        }
        playerSkills.skillNames = newSkillNames.ToArray();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedObject playerSkills = new SerializedObject(target as PlayerSkills);
        SerializedProperty skillNamesSP = playerSkills.FindProperty("skillNames");

        //PlayerSkills playerSkills = target as PlayerSkills;
        //if (playerSkills.skillNames == null) playerSkills.skillNames = new string[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < skillNamesSP.arraySize; i++)
        {
            EditorGUILayout.LabelField(skillNamesSP.GetArrayElementAtIndex(i).stringValue, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < skillNamesSP.arraySize; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            skillNamesSP.DeleteArrayElementAtIndex(killIndex);
            playerSkills.ApplyModifiedProperties();
            //fix?
            return;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        selectedSkillIndex = EditorGUILayout.Popup(selectedSkillIndex, this.skillNames.ToArray(), GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
        {
            string selectedSkillName = this.skillNames[selectedSkillIndex];
            selectedSkillIndex = -1;

            int targetIndex = skillNamesSP.arraySize;

            skillNamesSP.InsertArrayElementAtIndex(targetIndex);
            skillNamesSP.GetArrayElementAtIndex(targetIndex).stringValue = selectedSkillName;
            playerSkills.ApplyModifiedProperties();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;
    }
    
    //public override void OnInspectorGUI()
    //{
    //    EditorGUILayout.LabelField("Please use the Party Editor Window");
    //    if (GUILayout.Button("Open Window"))
    //    {
    //        PartyEditorWindow.MenuCreateWindow();
    //    }
    //}
}