using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PartyEditorWindow : EditorWindow
{

    public static PartyEditorWindow instance;

    string errorMessage = "";

    GameObject selectedGO = null;
    GameMaster gm;
    SerializedObject entityTarget;
    SerializedObject skillsTarget;
    SerializedObject serializedObject;

    string newDataName;

    int currentSelectedPopUpSkill = 0;
    int partySelected = 0;
    int oldSelected = -1;

    [MenuItem("RPG Framework/Party Editor")]
    public static void MenuCreateWindow()
    {
        if (instance == null) instance = CreateWindow<PartyEditorWindow>();
        else EditorWindow.FocusWindowIfItsOpen<PartyEditorWindow>();
        instance.UpdateSlection();
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
            //This is GameMaster
            serializedObject = new SerializedObject(selectedGO);
            gm = selectedGO.GetComponent<GameMaster>();

            //three types of editing - 
            ///Adding/removing party members
            ///Modifying stats or creating a new Entity data set
            ///Modifying skill
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Party");            
            EditParty();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Character", GUILayout.MinWidth(300));
            EditCharacter();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Skills", GUILayout.MinWidth(300));
            EditSkills();
            EditorGUILayout.EndVertical();
        }
    }

    public void EditParty()
    {
        //get part reference and find out how large it is
        Party partyComp = selectedGO.GetComponentInChildren<Party>();
        SerializedObject partySO = new SerializedObject(partyComp);
        SerializedProperty partyList = partySO.FindProperty("partyList");
        int partySize = partyList.arraySize;

        //validate partSelection and remember old value
        oldSelected = partySelected;
        if (partySelected >= partySize) partySelected = partySize - 1;

        //build string of party member names
        string[] names = new string[partySize];
        for(int i=0; i<partySize; i++)
        {
            Entity current = partyList.GetArrayElementAtIndex(i).objectReferenceValue as Entity;
            if (current == null)
            {
                names[i] = "No Entity Data";
                entityTarget = null;
            }
            else
            {
                names[i] = current.entityName;
                entityTarget = new SerializedObject(current);
            }
        }
        partySelected = GUILayout.SelectionGrid(partySelected, names, 1);
        //give some seperation
        GUILayout.Space(15);
        //allow for the deletion and addition of new party members
        if(GUILayout.Button("Delete Selected (no undo)"))
        {
            //odd behavior, it will not actually remove from the list if there is data,
            //it just deletes data.
            //So if there is data, we need to delete twice
            if (partyList.GetArrayElementAtIndex(partySelected).objectReferenceValue != null)
            {
                partyList.DeleteArrayElementAtIndex(partySelected);
                partyList.DeleteArrayElementAtIndex(partySelected);
            }
            else
            {
                partyList.DeleteArrayElementAtIndex(partySelected);
            }
        }
        if(GUILayout.Button("Add New"))
        {
            partyList.InsertArrayElementAtIndex(partySize);
            //need to set to null or it will duplicate the last element
            partyList.GetArrayElementAtIndex(partySize).objectReferenceValue = null;
        }

        //grab the currect selected party member
        if (partyComp.partyList[partySelected] != null)
        {
            entityTarget = new SerializedObject(gm.GetComponentInChildren<Party>().partyList[partySelected]);
        }
        else
        {
            entityTarget = null;
        }

        //save changes
        partySO.ApplyModifiedProperties();

        //check for a repaint
        if (partySelected != oldSelected)
        {
            //Need this to break focus to prevent 'ghost values' from confusing people.
            GUI.FocusControl("");
            Repaint();
        }
        
    }

    //edit a specific character
    public void EditCharacter()
    {

        //check for null data
        if(entityTarget == null)
        {
            EditorGUILayout.LabelField("Either create new entity data or assign existing data.");
            EditorGUILayout.LabelField("Don't assign monster data to players.");
            EditorGUILayout.BeginHorizontal();
            newDataName = EditorGUILayout.TextField("Data Name: ", newDataName);
            if (GUILayout.Button("Create Data"))
            {
                Entity temp = ScriptableObject.CreateInstance<Entity>() as Entity;
                temp.name = newDataName + "_data.asset";
                string targetPath = @"Assets\RPG System\Prefabs\Characters\CharacterData\" + temp.name;
                if (AssetDatabase.LoadAssetAtPath(targetPath, typeof(Entity)) == null)
                {
                    AssetDatabase.CreateAsset(temp, @"Assets\RPG System\Prefabs\Characters\CharacterData\" + temp.name);
                    //need to re-grab the part as serialized object
                    SerializedObject partySO = new SerializedObject(gm.GetComponentInChildren<Party>());
                    SerializedProperty partyList = partySO.FindProperty("partyList");
                    partyList.GetArrayElementAtIndex(partySelected).objectReferenceValue = temp;
                    partySO.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("Name clash with assets!  " + targetPath + " already exists! Use picker instead?");
                }

            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            SerializedProperty currentStat = entityTarget.FindProperty("entityName");
            currentStat.stringValue = EditorGUILayout.TextField("Name: ", currentStat.stringValue);

            EditorGUILayout.LabelField("Battle ID is used to link the prefab");
            EditorGUILayout.LabelField("placed in battle scenes to the stats.");
            EditorGUILayout.LabelField("It must match exactly.");

            currentStat = entityTarget.FindProperty("combatID");
            currentStat.stringValue = EditorGUILayout.TextField("Battle ID: ", currentStat.stringValue);

            currentStat = entityTarget.FindProperty("strengthBase");
            currentStat.intValue = EditorGUILayout.IntField("Strength", currentStat.intValue);

            currentStat = entityTarget.FindProperty("constitutionBase");
            currentStat.intValue = EditorGUILayout.IntField("Constitution", currentStat.intValue);

            currentStat = entityTarget.FindProperty("intelligenceBase");
            currentStat.intValue = EditorGUILayout.IntField("Intelligence", currentStat.intValue);

            currentStat = entityTarget.FindProperty("willpowerBase");
            currentStat.intValue = EditorGUILayout.IntField("Willpower", currentStat.intValue);

            currentStat = entityTarget.FindProperty("physicalArmorBase");
            currentStat.intValue = EditorGUILayout.IntField("Physical Armor", currentStat.intValue);

            currentStat = entityTarget.FindProperty("magicArmorBase");
            currentStat.intValue = EditorGUILayout.IntField("Magic Armor", currentStat.intValue);

            currentStat = entityTarget.FindProperty("entityLevel");
            currentStat.intValue = EditorGUILayout.IntField("Level", currentStat.intValue);

            //All done editing
            //calculate stats
            Entity currentEntity = gm.GetComponentInChildren<Party>().partyList[partySelected];
            currentEntity.ResetStatsToBase();
            currentEntity.CalculateAllStats();
            currentEntity.FullHeal();
            entityTarget.ApplyModifiedProperties();

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

    public void EditSkills()
    {
        EditorGUILayout.LabelField("Not working yet.");
        EditorGUILayout.LabelField("Select prefab and manually enter skill names.");
        return;
        //grab player skills from Entity
        Entity currentEntity = gm.GetComponentInChildren<Party>().partyList[partySelected];
        if (currentEntity.playerSkills == null) currentEntity.playerSkills = new PlayerSkills();
        SerializedObject skills = new SerializedObject(entityTarget.FindProperty("playerSkills").objectReferenceValue);
        //Grab skill names
        SerializedProperty skillList = skills.FindProperty("skillNames");
        if (!SkillDatabase.isLoaded) SkillDatabase.LoadSkills();

        int skillCount = skillList.arraySize;
        int deleteTarget = -1;
        for (int i = 0; i < skillCount; i++)
        {
            SerializedProperty currentSkillName = skillList.GetArrayElementAtIndex(i);
            SkillDescriptor currentSkill = SkillDatabase.GetSkill(currentSkillName.stringValue);
            GUILayout.BeginHorizontal();

            GUILayout.Label(currentSkill.displayText);
            if (GUILayout.Button("Remove"))
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
        for (int i = 0; i < allSkills.Count; i++)
        {
            skillDisplay[i] = allSkills[i].displayText;
        }

        GUILayout.BeginHorizontal();
        currentSelectedPopUpSkill = EditorGUILayout.Popup(currentSelectedPopUpSkill, skillDisplay);
        if (GUILayout.Button("Add"))
        {
            int targetIndex = skillList.arraySize;
            skillList.InsertArrayElementAtIndex(targetIndex);
            SerializedProperty newSkill = skillList.GetArrayElementAtIndex(targetIndex);
            newSkill.stringValue = allSkills[currentSelectedPopUpSkill].name;
        }

        skills.ApplyModifiedProperties();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void CodeReload()
    {
        //detects if the window is alread opened and grabs it.
        //Code recompiles causes the instance reference to be lost.
        if (EditorWindow.HasOpenInstances<PartyEditorWindow>())
        {
            instance = EditorWindow.GetWindow<PartyEditorWindow>("", false);
            instance.UpdateSlection();
        }
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
            errorMessage = "Open the Core scene and open the GameMaster GO in the prefab editor.";
        }
        else if (selectedGO.scene.name == null)
        {
            errorMessage = "Open the Core scene and open the GameMaster GO in the prefab editor.";
            selectedGO = null;
        }
        else if (selectedGO.scene.name == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name)
        {
            errorMessage = "Open the Core scene and open the GameMaster GO in the prefab editor.";
            selectedGO = null;
        }
        else if (selectedGO.GetComponent<GameMaster>() == null)
        {
            errorMessage = "GameMaster not selected.";
            selectedGO = null;
        }
        else
        {
            //Valid selection, setup the Serialized objects for editing
            //skillsTarget = new SerializedObject(selectedGO.GetComponent<EnemySkills>());
            //entityTarget = new SerializedObject(selectedGO.GetComponent<EnemyCombatController>());
        }

        Repaint();
    }

}
