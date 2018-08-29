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
        UnityEngine.MonoBehaviour.print(skillNames.Count.ToString() + " skills loaded");

        EnemySkills enemySkills = target as EnemySkills;
        List<WeightedSkill> invalidSkills = new List<WeightedSkill>();
        foreach (WeightedSkill descriptor in enemySkills.weightedSkills)
        {
            if (skillNames.Contains(descriptor.skill.name))
            {
                skillNames.Remove(descriptor.skill.name);
            }
            else
            {
                invalidSkills.Add(descriptor);
            }
        }
        List<WeightedSkill> newSkills = new List<WeightedSkill>();
        newSkills.AddRange(enemySkills.weightedSkills);
        foreach (WeightedSkill invalidSkill in invalidSkills)
        {
            newSkills.Remove(invalidSkill);
        }
        enemySkills.weightedSkills = newSkills.ToArray();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemySkills enemySkills = target as EnemySkills;
        if (enemySkills.weightedSkills == null) enemySkills.weightedSkills = new WeightedSkill[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.weightedSkills.Length; i++)
        {
            EditorGUILayout.LabelField(enemySkills.weightedSkills[i].skill.name, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        EditorGUILayout.LabelField("Weight", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        for (int i = 0; i < enemySkills.weightedSkills.Length; i++)
        {
            enemySkills.weightedSkills[i].weight = EditorGUILayout.IntField(enemySkills.weightedSkills[i].weight, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(40));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.weightedSkills.Length; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            // Rebuild array without this one
            List<WeightedSkill> newSkills = new List<WeightedSkill>();
            for (int i = 0; i < enemySkills.weightedSkills.Length; i++)
            {
                if (i != killIndex) newSkills.Add(enemySkills.weightedSkills[i]);
            }
            enemySkills.weightedSkills = newSkills.ToArray();
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

            List<WeightedSkill> newSkills = new List<WeightedSkill>();
            newSkills.AddRange(enemySkills.weightedSkills);
            newSkills.Add(WeightedSkill.Create(addedSkill, 1));

            enemySkills.weightedSkills = newSkills.ToArray();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;



    }
}
