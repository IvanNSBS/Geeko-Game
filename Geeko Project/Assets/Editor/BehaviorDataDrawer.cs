using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(BehaviorData))]
public class BehaviorDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect lb = new Rect(position.x, position.y, 30, position.height);
        Rect amountRect = new Rect(position.x, position.y+20, 30, position.height);

        //EditorGUI.LabelField(lb, "Custom Label!");
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("m_Strength"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
