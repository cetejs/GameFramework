using System.Collections.Generic;
using GameFramework.DelayedActionService;
using UnityEditor;

namespace GameFramework
{
    [CustomEditor(typeof(DelayedActionManager))]
    internal class DelayedActionManagerEditor : Editor
    {
        private DelayedActionManager manager;
        private readonly List<DelegateInfo> delegateInfos = new List<DelegateInfo>();
        private bool isFoldout;
        private int lastCount;

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
                    string message = info.ToString();
                    EditorGUILayout.LabelField(message);
                }
            }
        }
    }
}