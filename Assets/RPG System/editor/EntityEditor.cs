using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EntityEditor : EditorWindow
{
    public static EntityEditor instance;

    string errorMessage = "";

    GameObject selectedGO = null;
    SerializedObject entityTarget;
    SerializedObject skillsTarget;

    int currentSelectedPopUpSkill = 0;

    [MenuItem("RPG Framework/Monster Editor")]
    public static void MenuCreateWindow()
    {
        if (instance == null) instance = CreateWindow<EntityEditor>();
        else EditorWindow.FocusWindowIfItsOpen<EntityEditor>();
        instance.UpdateSlection();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void CodeReload()
    {
        //detects if the window is alread opened and grabs it.
        //Code recompiles causes the instance reference to be lost.
        if (EditorWindow.HasOpenInstances<EntityEditor>())
        {
            instance = EditorWindow.GetWindow<EntityEditor>("", false);
            instance.UpdateSlection();
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleRight;

        //invalid selection or prefab editor not open
        if (selectedGO == null)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(errorMessage);
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical();
            EditEntity();           
            EditSkills();
            GUILayout.EndVertical();
        }
    }

    private void EditEntity()
    {
        EnemyCombatController ecc = Selection.activeGameObject.GetComponent<EnemyCombatController>();
        Entity temp = ecc.entity;
        if (temp == null)
        {
            GUILayout.Label("No monster data found.  Create new data or assign existing.");
            if(GUILayout.Button("Create Data"))
            {
                temp = ScriptableObject.CreateInstance<Entity>() as Entity;
                temp.name = Selection.activeGameObject.name + "_data.asset";
                string targetPath = @"Assets\RPG System\Prefabs\Monsters\MonsterData\" + temp.name;
                if (AssetDatabase.LoadAssetAtPath(targetPath, typeof(Entity)) == null)
                {
                    AssetDatabase.CreateAsset(temp, @"Assets\RPG System\Prefabs\Monsters\MonsterData\" + temp.name);
                    SerializedObject eccAsset = new SerializedObject(ecc);
                    SerializedProperty eccEntity = eccAsset.FindProperty("entity");
                    eccEntity.objectReferenceValue = temp;
                    eccAsset.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("Name clash with assets!  " + targetPath + " already exists! Use picker instead?");
                }

            }
        }
        else
        {                        
            SerializedObject eccAsset = new SerializedObject(Selection.activeGameObject.GetComponent<EnemyCombatController>());
            SerializedProperty monsterDataAsset = eccAsset.FindProperty("entity");
            monsterDataAsset.objectReferenceValue = EditorGUILayout.ObjectField("Data:", monsterDataAsset.objectReferenceValue, typeof(Entity), false);

            Entity currentEntity = monsterDataAsset.objectReferenceValue as Entity;
            SerializedObject monsterStats = new SerializedObject(currentEntity);

            SerializedProperty currentStat = monsterStats.FindProperty("strengthBase");
            currentStat.intValue = EditorGUILayout.IntField("Strength", currentStat.intValue);

            currentStat = monsterStats.FindProperty("constitutionBase");
            currentStat.intValue = EditorGUILayout.IntField("Constitution", currentStat.intValue);

            currentStat = monsterStats.FindProperty("intelligenceBase");
            currentStat.intValue = EditorGUILayout.IntField("Intelligence", currentStat.intValue);

            currentStat = monsterStats.FindProperty("willpowerBase");
            currentStat.intValue = EditorGUILayout.IntField("Willpower", currentStat.intValue);

            currentStat = monsterStats.FindProperty("physicalArmorBase");
            currentStat.intValue = EditorGUILayout.IntField("Physical Armor", currentStat.intValue);

            currentStat = monsterStats.FindProperty("magicArmorBase");
            currentStat.intValue = EditorGUILayout.IntField("Magic Armor", currentStat.intValue);

            currentStat = monsterStats.FindProperty("entityLevel");
            currentStat.intValue = EditorGUILayout.IntField("Level", currentStat.intValue);

            //All done editing
            currentEntity.ResetStatsToBase();
            currentEntity.CalculateAllStats();
            currentEntity.FullHeal();
            eccAsset.ApplyModifiedProperties();
            monsterStats.ApplyModifiedProperties();

            //Display Read-only properties
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Calculated Properties");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField("HP: " + currentEntity.hitPoints);
            EditorGUILayout.LabelField("Endurance: " + currentEntity.endurance);
            EditorGUILayout.LabelField("Mana: " + currentEntity.mana);
            EditorGUI.indentLevel -= 1;
        }
    }

    private void EditSkills()
    {

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Skills");
        EditorGUILayout.LabelField("To give a skill a higher chance of being");
        EditorGUILayout.LabelField("used add it in multiple times.");
        EditorGUILayout.Space();

        SerializedObject skills = new SerializedObject(Selection.activeGameObject.GetComponent<EnemySkills>());
        SerializedProperty skillList = skills.FindProperty("skillNames");
        if (!SkillDatabase.isLoaded) SkillDatabase.LoadSkills();

        int skillCount = skillList.arraySize;
        int deleteTarget = -1;
        for(int i=0; i<skillCount; i++)
        {            
            SerializedProperty currentSkillName = skillList.GetArrayElementAtIndex(i);
            SkillDescriptor currentSkill = SkillDatabase.GetSkill(currentSkillName.stringValue);
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(currentSkill.displayText);
            if(GUILayout.Button("Remove"))
            {
                deleteTarget = i;
            }
            GUILayout.EndHorizontal();
        }
        if (deleteTarget > -1)
        {
            skillList.DeleteArrayElementAtIndex(deleteTarget);
        }

        //put all skills into a combo box for selection
        List<SkillDescriptor> allSkills = SkillDatabase.GetAllSkills();
        string[] skillDisplay = new string[allSkills.Count];
        for(int i=0; i<allSkills.Count; i++)
        {
            skillDisplay[i] = allSkills[i].displayText;
        }

        GUILayout.BeginHorizontal();
        currentSelectedPopUpSkill = EditorGUILayout.Popup(currentSelectedPopUpSkill, skillDisplay);
        if(GUILayout.Button("Add"))
        {
            int targetIndex = skillList.arraySize;
            skillList.InsertArrayElementAtIndex(targetIndex);
            SerializedProperty newSkill = skillList.GetArrayElementAtIndex(targetIndex);
            newSkill.stringValue = allSkills[currentSelectedPopUpSkill].name;
        }

        skills.ApplyModifiedProperties();
    }


    private void OnSelectionChange()
    {
        UpdateSlection();
    }

    private void UpdateSlection()
    {
        //clear error and selected
        errorMessage = string.Empty;
        selectedGO = Selection.activeGameObject;

        //validate that the prefab editor is open
        if (selectedGO == null)
        {
            errorMessage = "Nothing Selected";
        }
        else if (selectedGO.scene.name == null)
        {
            errorMessage = "Open the prefeb editor window";
            selectedGO = null;
        }
        else if (selectedGO.scene.name == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name)
        {
            errorMessage = "Open the the prefab editor window";
            selectedGO = null;
        }
        else if (selectedGO.GetComponent<EnemyCombatController>() == null)
        {
            errorMessage = "No Enemy Combat Controller Detected";
            selectedGO = null;
        }
        else
        {
            //Valid selection, setup the Serialized objects for editing
            skillsTarget = new SerializedObject(selectedGO.GetComponent<EnemySkills>());
            entityTarget = new SerializedObject(selectedGO.GetComponent<EnemyCombatController>());
        }

        Repaint();
    }
}

