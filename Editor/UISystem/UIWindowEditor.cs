using GameFramework.UIService;
using UnityEditor;

namespace GameFramework
{
    [CustomEditor(typeof(UIWindow), true)]
    internal class UIWindowEditor : Editor
    {
        private SerializedProperty layerProperty;

        private void OnEnable()
        {
            layerProperty = serializedObject.FindProperty("layer");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "layer")
                {
                    layerProperty.intValue = EditorGUILayout.Popup("Layer", layerProperty.intValue, UIConfig.Get().windowLayers);
                }
                else
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}