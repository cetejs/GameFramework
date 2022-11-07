using System.Collections.Generic;
using GameFramework.Generic;
using UnityEditor;

namespace GameFramework
{
    [CustomEditor(typeof(ReferencePoolInfo))]
    internal class ReferencePoolInfoEditor : Editor
    {
        private ReferencePoolInfo poolInfo;
        private readonly List<string> debugInfos = new List<string>();
        private bool isFoldout;

        private void OnEnable()
        {
            poolInfo = target as ReferencePoolInfo;
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            isFoldout = EditorGUILayout.Foldout(isFoldout, "Reference Pool Info");
            if (isFoldout)
            {
                poolInfo.GetDebugInfos(debugInfos);
                for (int i = 0; i < debugInfos.Count; i++)
                {
                    EditorGUILayout.LabelField(debugInfos[i]);
                }
            }
        }
    }
}