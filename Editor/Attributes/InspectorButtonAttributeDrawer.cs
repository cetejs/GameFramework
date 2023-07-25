using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    internal class InspectorButtonAttributeDrawer : SupportReadOnlyDrawer
    {
        private MethodInfo methodInfo;

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorButtonAttribute inspectorButton = (InspectorButtonAttribute) attribute;
            object targetObject = property.serializedObject.targetObject;
            string buttonName = inspectorButton.MethodName;

            if (methodInfo == null)
            {
                Type type = targetObject.GetType();
                methodInfo = type.GetMethod(inspectorButton.MethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
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