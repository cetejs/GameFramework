using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public abstract class ManagerInfoInspector : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Label(target.ToString());
        }
    }
}