using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Entity))]
public class EntityInspector : PropertyDrawer {

    SerializedProperty Strength;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        EditorGUILayout.LabelField(property.FindPropertyRelative("name").stringValue);
    }

}
