using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal static class EditorHelper
    {
        public static void HandleClickSelection(GameObject gameObject, Event evt)
        {
            if (evt.shift || EditorGUI.actionKey)
            {
                Object[] existingSelection = Selection.objects;

                // For shift, we check if EXACTLY the active GO is hovered by mouse and then subtract. Otherwise additive.
                // For control/cmd, we check if ANY of the selected GO is hovered by mouse and then subtract. Otherwise additive.
                // Control/cmd takes priority over shift.
                bool subtractFromSelection = EditorGUI.actionKey ? Selection.Contains(gameObject) : Selection.activeGameObject == gameObject;
                if (subtractFromSelection)
                {
                    // subtract from selection
                    Object[] newSelection = new UnityEngine.Object[existingSelection.Length - 1];

                    int index = Array.IndexOf(existingSelection, gameObject);

                    Array.Copy(existingSelection, newSelection, index);
                    Array.Copy(existingSelection, index + 1, newSelection, index, newSelection.Length - index);

                    Selection.objects = newSelection;
                }
                else
                {
                    // add to selection
                    Object[] newSelection = new UnityEngine.Object[existingSelection.Length + 1];
                    Array.Copy(existingSelection, newSelection, existingSelection.Length);
                    newSelection[existingSelection.Length] = gameObject;

                    Selection.objects = newSelection;
                }
            }
            else
                Selection.activeObject = gameObject;
        }
    }
}