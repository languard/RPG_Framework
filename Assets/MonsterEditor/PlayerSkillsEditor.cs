using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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

        PlayerSkills playerSkills = target as PlayerSkills;
        if (playerSkills.skillNames == null) playerSkills.skillNames = new string[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < playerSkills.skillNames.Length; i++)
        {
            EditorGUILayout.LabelField(playerSkills.skillNames[i], GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < playerSkills.skillNames.Length; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            // Rebuild array without this one
            List<string> newSkills = new List<string>();
            for (int i = 0; i < playerSkills.skillNames.Length; i++)
            {
                if (i != killIndex) newSkills.Add(playerSkills.skillNames[i]);
            }
            playerSkills.skillNames = newSkills.ToArray();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        selectedSkillIndex = EditorGUILayout.Popup(selectedSkillIndex, skillNames.ToArray(), GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
        {
            string selectedSkillName = skillNames[selectedSkillIndex];
            selectedSkillIndex = -1;

            List<string> newSkills = new List<string>();
            newSkills.AddRange(playerSkills.skillNames);
            newSkills.Add(selectedSkillName);

            playerSkills.skillNames = newSkills.ToArray();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;
    }
}
#endif