using System;
using System.Collections.Generic;
using UnityEditor;
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
        List<SkillDescriptor> invalidSkills = new List<SkillDescriptor>();
        if (playerSkills.skills == null) playerSkills.skills = new SkillDescriptor[0];
        foreach (SkillDescriptor descriptor in playerSkills.skills)
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
        newSkills.AddRange(playerSkills.skills);
        foreach (SkillDescriptor invalidSkill in invalidSkills)
        {
            newSkills.Remove(invalidSkill);
        }
        playerSkills.skills = newSkills.ToArray();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PlayerSkills playerSkills = target as PlayerSkills;
        if (playerSkills.skills == null) playerSkills.skills = new SkillDescriptor[0];

        int y = 0, killIndex = -1;
        //EditorGUI.LabelField(new Rect(5, y, 50, 15), "Skill");
        //EditorGUI.LabelField(new Rect(100, y, 50, 15), "Weight");

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Skill", GUILayout.ExpandWidth(false));
        for (int i = 0; i < playerSkills.skills.Length; i++)
        {
            EditorGUILayout.LabelField(playerSkills.skills[i].name, GUILayout.ExpandWidth(false));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Remove", GUILayout.ExpandWidth(false));
        for (int i = 0; i < playerSkills.skills.Length; i++)
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
            for (int i = 0; i < playerSkills.skills.Length; i++)
            {
                if (i != killIndex) newSkills.Add(playerSkills.skills[i]);
            }
            playerSkills.skills = newSkills.ToArray();
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
            newSkills.AddRange(playerSkills.skills);
            newSkills.Add(addedSkill);

            playerSkills.skills = newSkills.ToArray();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        return;



    }
}
