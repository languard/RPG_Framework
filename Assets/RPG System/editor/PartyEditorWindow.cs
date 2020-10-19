using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PartyEditorWindow : EditorWindow
{

    public static PartyEditorWindow instance;

    string errorMessage = "";

    GameObject selectedGO = null;
    SerializedObject entityTarget;
    SerializedObject skillsTarget;

    int currentSelectedPopUpSkill = 0;

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
            GUILayout.BeginVertical();
            //EditEntity();
            //EditSkills();
            GUILayout.EndVertical();
        }
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
