﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySkills))]
public class EnemySkillsEditor : Editor
{
    private List<string> skillNames = new List<string>();
    private int selectedSkillIndex = 0;

    private void OnEnable()
    {
        // Reload skills database
        SkillDatabase.LoadSkills();
        skillNames = SkillDatabase.FindSkills(string.Empty);

        EnemySkills enemySkills = target as EnemySkills;
        List<SkillDescriptor> invalidSkills = new List<SkillDescriptor>();
        if (enemySkills.skills == null) enemySkills.skills = new SkillDescriptor[0];
        if (enemySkills.weights == null) enemySkills.weights = new int[0];
        foreach (SkillDescriptor descriptor in enemySkills.skills)
        {
            if (skillNames.Contains(descriptor.name))
            {
                skillNames.Remove(descriptor.name);
            }
            else
            {
                invalidSkills.Add(descriptor);
            }
        }
        List<SkillDescriptor> newSkills = new List<SkillDescriptor>();
        newSkills.AddRange(enemySkills.skills);
        foreach (SkillDescriptor invalidSkill in invalidSkills)
        {
            newSkills.Remove(invalidSkill);
        }
        enemySkills.skills = newSkills.ToArray();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemySkills enemySkills = target as EnemySkills;
        if (enemySkills.skills == null) enemySkills.skills = new SkillDescriptor[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.skills.Length; i++)
        {
            EditorGUILayout.LabelField(enemySkills.skills[i].name, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        EditorGUILayout.LabelField("Weight", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        for (int i = 0; i < enemySkills.skills.Length; i++)
        {
            enemySkills.weights[i] = EditorGUILayout.IntField(enemySkills.weights[i], GUILayout.ExpandWidth(false), GUILayout.MaxWidth(40));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.skills.Length; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            // Rebuild array without this one
            List<SkillDescriptor> newSkills = new List<SkillDescriptor>();
            List<int> newWeights = new List<int>();
            for (int i = 0; i < enemySkills.skills.Length; i++)
            {
                if (i != killIndex)
                {
                    newSkills.Add(enemySkills.skills[i]);
                    newWeights.Add(enemySkills.weights[i]);
                }
            }
            enemySkills.skills = newSkills.ToArray();
            enemySkills.weights = newWeights.ToArray();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        selectedSkillIndex = EditorGUILayout.Popup(selectedSkillIndex, skillNames.ToArray(), GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
        {
            string selectedSkillName = skillNames[selectedSkillIndex];
            selectedSkillIndex = -1;
            SkillDescriptor addedSkill = SkillDatabase.GetSkill(selectedSkillName);

            List<SkillDescriptor> newSkills = new List<SkillDescriptor>();
            newSkills.AddRange(enemySkills.skills);
            newSkills.Add(addedSkill);
            enemySkills.skills = newSkills.ToArray();

            List<int> newWeights = new List<int>();
            newWeights.AddRange(enemySkills.weights);
            newWeights.Add(1);
            enemySkills.weights = newWeights.ToArray();

        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;



    }
}
