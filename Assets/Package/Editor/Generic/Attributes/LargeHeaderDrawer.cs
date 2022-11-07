using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(LargeHeaderAttribute))]
    internal class LargeHeaderDrawer : DecoratorDrawer
    {
        private GUIStyle style;

        public override float GetHeight()
        {
            return base.GetHeight() * 2.0f;
        }

        public override void OnGUI(Rect pos)
        {
            LargeHeaderAttribute header = (LargeHeaderAttribute) attribute;
            if (!ColorUtility.TryParseHtmlString(header.color, out Color color))
            {
                color = Color.white;
            }

            color *= header.alpha;
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = Mathf.Max(style.fontSize, header.size);
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.LowerLeft;
            GUI.color = color;
            EditorGUI.LabelField(pos, header.name, style);
            GUI.color = Color.white;
        }
    }
}