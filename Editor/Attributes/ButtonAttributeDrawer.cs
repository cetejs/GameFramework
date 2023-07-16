using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    internal class InspectorButtonAttributeDrawer : SupportReadOnlyDrawer
    {
        private MethodInfo methodInfo;

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorGroupAttribute group = fieldInfo.GetCustomAttribute<InspectorGroupAttribute>();
            if (group != null)
            {
                position.x += 10;
            }

            ButtonAttribute button = (ButtonAttribute) attribute;
            object targetObject = property.serializedObject.targetObject;
            string buttonName = button.MethodName;

            if (methodInfo == null)
            {
                Type type = targetObject.GetType();
                methodInfo = type.GetMethod(button.MethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            }

            if (methodInfo == null)
            {
                buttonName += "(Error)";
            }

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
            };

            if (GUI.Button(position, buttonName, style))
            {
                if (methodInfo != null)
                {
                    methodInfo.Invoke(targetObject, null);
                }
            }
        }
    }
}