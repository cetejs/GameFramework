using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class ObjectPoolWindow : SubWindow
    {
        private Editor settingEditor;

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }
        }

        public override void Init(string name, GameWindow parent)
        {
            base.Init("ObjectPool", parent);
            settingEditor = Editor.CreateEditor(ObjectPoolSetting.Instance);
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }
    }
}