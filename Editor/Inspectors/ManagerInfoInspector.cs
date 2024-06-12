using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal abstract class ManagerInfoInspector : Editor
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