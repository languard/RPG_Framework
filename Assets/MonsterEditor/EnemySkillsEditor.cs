using System;
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
        List<string> invalidSkillNames = new List<string>();
        if (enemySkills.skillNames == null) enemySkills.skillNames = new string[0];
        if (enemySkills.weights == null) enemySkills.weights = new int[0];
        foreach (string skillName in enemySkills.skillNames)
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
        newSkillNames.AddRange(enemySkills.skillNames);
        foreach (string invalidSkill in invalidSkillNames)
        {
            newSkillNames.Remove(invalidSkill);
        }
        enemySkills.skillNames = newSkillNames.ToArray();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemySkills enemySkills = target as EnemySkills;
        if (enemySkills.skillNames == null) enemySkills.skillNames = new string[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.skillNames.Length; i++)
        {
            EditorGUILayout.LabelField(enemySkills.skillNames[i], GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        EditorGUILayout.LabelField("Weight", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        for (int i = 0; i < enemySkills.skillNames.Length; i++)
        {
            enemySkills.weights[i] = EditorGUILayout.IntField(enemySkills.weights[i], GUILayout.ExpandWidth(false), GUILayout.MaxWidth(40));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.skillNames.Length; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            // Rebuild array without this one
            List<string> newSkillNames = new List<string>();
            List<int> newWeights = new List<int>();
            for (int i = 0; i < enemySkills.skillNames.Length; i++)
            {
                if (i != killIndex)
                {
                    newSkillNames.Add(enemySkills.skillNames[i]);
                    newWeights.Add(enemySkills.weights[i]);
                }
            }
            enemySkills.skillNames = newSkillNames.ToArray();
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

            List<string> newSkills = new List<string>();
            newSkills.AddRange(enemySkills.skillNames);
            newSkills.Add(selectedSkillName);
            enemySkills.skillNames = newSkills.ToArray();

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
