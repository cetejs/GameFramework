using UnityEditor;

namespace GameFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameScriptableObject), true, isFallback = true)]
    internal class GameScriptableObjectInspector : GameBehaviourInspector
    {
    }
}