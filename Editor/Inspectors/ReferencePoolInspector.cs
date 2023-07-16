using System.Collections.Generic;
using UnityEditor;

namespace GameFramework
{
    [CustomEditor(typeof(ReferencePool))]
    internal class ReferencePoolInspector : Editor
    {
        private ReferencePool pool;
        private List<string> poolInfos = new List<string>();
        private bool isFoldout = true;

        private void OnEnable()
        {
            pool = target as ReferencePool;
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
                pool.GetPoolInfos(poolInfos);
                foreach (string info in poolInfos)
                {
                    EditorGUILayout.LabelField(info);
                }
            }
        }
    }
}