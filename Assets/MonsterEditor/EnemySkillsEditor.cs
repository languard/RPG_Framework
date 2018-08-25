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
        List<SkillDescriptor> invalidSkills = new List<SkillDescriptor>();
        foreach (SkillDescriptor descriptor in enemySkills.Skills)
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
        foreach (SkillDescriptor invalidSkill in invalidSkills)
        {
            enemySkills.Skills.Remove(invalidSkill);
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemySkills enemySkills = target as EnemySkills;

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.Skills.Count; i++)
        {
            EditorGUILayout.LabelField(enemySkills.Skills[i].name, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        EditorGUILayout.LabelField("Weight", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));
        for (int i = 0; i < enemySkills.Skills.Count; i++)
        {
            enemySkills.Weights[i] = EditorGUILayout.IntField(enemySkills.Weights[i], GUILayout.ExpandWidth(false), GUILayout.MaxWidth(40));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < enemySkills.Skills.Count; i++)
        {
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                killIndex = i;
            }
        }
        if (killIndex >= 0)
        {
            enemySkills.Weights.RemoveAt(killIndex);
            enemySkills.Skills.RemoveAt(killIndex);
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
            enemySkills.Skills.Add(addedSkill);
            enemySkills.Weights.Add(1);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;



    }
}
