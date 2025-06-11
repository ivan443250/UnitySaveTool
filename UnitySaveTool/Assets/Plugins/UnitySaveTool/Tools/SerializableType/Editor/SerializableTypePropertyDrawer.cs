using System;
using UnityEditor;
using UnityEngine;

namespace UnitySaveTool.Tools
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty mainProperty, GUIContent label)
        {
            SerializedProperty scriptProperty = mainProperty.FindPropertyRelative(SerializableType.ScriptField);

            UnityEngine.Object scriptObj = scriptProperty.objectReferenceValue;
            scriptObj = EditorGUI.ObjectField(position, label, scriptObj, typeof(MonoScript), false);

            scriptProperty.objectReferenceValue = scriptObj;

            SetClassInfo(mainProperty, scriptObj);
        }

        private void SetClassInfo(SerializedProperty mainProperty, UnityEngine.Object scriptObj)
        {
            if (scriptObj is not MonoScript script)
                return;

            Type type = script.GetClass();

            SerializedProperty classInfoProperty = mainProperty.FindPropertyRelative(SerializableType.ClassInfoField);

            classInfoProperty.stringValue = $"{type.FullName}, {type.Assembly.FullName}";
        }
    }
}
