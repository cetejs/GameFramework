using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomEditor(typeof(BoneRenderer))]
    [CanEditMultipleObjects]
    internal class BoneRendererInspector : Editor
    {
        private static readonly GUIContent k_BoneSizeLabel = new GUIContent("Bone Size");
        private static readonly GUIContent k_BoneColorLabel = new GUIContent("Color");
        private static readonly GUIContent k_BoneShapeLabel = new GUIContent("Shape");
        private static readonly GUIContent k_TripodSizeLabel = new GUIContent("Tripod Size");

        private SerializedProperty m_DrawBones;
        private SerializedProperty m_BoneShape;
        private SerializedProperty m_BoneSize;
        private SerializedProperty m_BoneColor;

        private SerializedProperty m_DrawTripods;
        private SerializedProperty m_TripodSize;

        private SerializedProperty m_Transforms;

        public void OnEnable()
        {
            m_DrawBones = serializedObject.FindProperty("drawBones");
            m_BoneSize = serializedObject.FindProperty("boneSize");
            m_BoneShape = serializedObject.FindProperty("boneShape");
            m_BoneColor = serializedObject.FindProperty("boneColor");

            m_DrawTripods = serializedObject.FindProperty("drawTripods");
            m_TripodSize = serializedObject.FindProperty("tripodSize");

            m_Transforms = serializedObject.FindProperty("m_Transforms");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_DrawBones, k_BoneSizeLabel);
            using (new EditorGUI.DisabledScope(!m_DrawBones.boolValue))
                EditorGUILayout.PropertyField(m_BoneSize, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(!m_DrawBones.boolValue))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_BoneShape, k_BoneShapeLabel);
                EditorGUILayout.PropertyField(m_BoneColor, k_BoneColorLabel);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_DrawTripods, k_TripodSizeLabel);
            using (new EditorGUI.DisabledScope(!m_DrawTripods.boolValue))
                EditorGUILayout.PropertyField(m_TripodSize, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Transforms, true);
            bool boneRendererDirty = EditorGUI.EndChangeCheck();

            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
                boneRendererDirty = true;

            serializedObject.ApplyModifiedProperties();

            if (boneRendererDirty)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    BoneRenderer boneRenderer = targets[i] as BoneRenderer;
                    boneRenderer.ExtractBones();
                }
            }
        }
    }
}