using System.Collections.Generic;
using UnityEditor;

namespace GameFramework
{
    [CustomEditor(typeof(DelayedActionManager))]
    internal class DelayedActionInspector : Editor
    {
        private DelayedActionManager manager;
        private readonly List<DelegateInfo> delegateInfos = new List<DelegateInfo>();
        private bool isFoldout = true;

        private void OnEnable()
        {
            manager = target as DelayedActionManager;
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            isFoldout = EditorGUILayout.Foldout(isFoldout, "Delegate Info");
            if (isFoldout)
            {
                manager.GetDelegateInfos(delegateInfos);
                foreach (DelegateInfo info in delegateInfos)
                {
                    EditorGUILayout.LabelField(info.ToString());
                }
            }
        }
    }
}