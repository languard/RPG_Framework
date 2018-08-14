#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[InitializeOnLoad]
public class SaveOnRun
{
    static SaveOnRun()
    {
        EditorApplication.playModeStateChanged += StateChange;
    }

    private static void StateChange(PlayModeStateChange state)
    {
        Debug.Log(state);
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("Saving all open scenes");
            EditorSceneManager.SaveOpenScenes();
        }
    }
}
#endif