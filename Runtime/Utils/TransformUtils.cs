using UnityEngine;

namespace GameFramework
{
    public static class TransformUtils
    {
        public static void ResetLocal(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public static void AdjustAnchor(this RectTransform rectTransform, AnchorLeftType leftType, AnchorTopType topType)
        {
            if (leftType == AnchorLeftType.Stretch || topType == AnchorTopType.Stretch)
            {
                if (leftType != AnchorLeftType.Stretch)
                {
                    float leftValue = GetAnchorLeftValue(leftType);
                    rectTransform.anchorMin = new Vector2(0f, leftValue);
                    rectTransform.anchorMax = new Vector2(1f, leftValue);
                    rectTransform.pivot = new Vector2(0.5f, leftValue);
                }
                else if (topType != AnchorTopType.Stretch)
                {
                    float topValue = GetAnchorTopValue(topType);
                    rectTransform.anchorMin = new Vector2(topValue, 0f);
                    rectTransform.anchorMax = new Vector2(topValue, 1f);
                    rectTransform.pivot = new Vector2(topValue, 0.5f);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                }

                rectTransform.offsetMin = new Vector2(0f, 0f);
                rectTransform.offsetMax = new Vector2(0f, 0f);
            }
            else
            {
                float leftValue = GetAnchorLeftValue(leftType);
                float topValue = GetAnchorTopValue(topType);
                rectTransform.anchorMin = new Vector2(topValue, leftValue);
                rectTransform.anchorMax = new Vector2(topValue, leftValue);
                rectTransform.pivot = new Vector2(topValue, leftValue);
            }
        }

        private static float GetAnchorLeftValue(AnchorLeftType type)
        {
            switch (type)
            {
                case AnchorLeftType.Top:
                    return 1f;
                case AnchorLeftType.Middle:
                    return 0.5f;
                case AnchorLeftType.Bottom:
                    return 0f;
            }

            return -1f;
        }

        private static float GetAnchorTopValue(AnchorTopType type)
        {
            switch (type)
            {
                case AnchorTopType.Left:
                    return 0f;
                case AnchorTopType.Center:
                    return 0.5f;
                case AnchorTopType.Right:
                    return 1f;
            }

            return -1f;
        }

        public static bool IsInScreen(Camera cam, Vector3 worldPoint)
        {
            Vector3 viewPoint = cam.worldToCameraMatrix.MultiplyPoint3x4(worldPoint);
            if (viewPoint.z > 0f)
            {
                return false;
            }

            Vector3 screenPoint = cam.WorldToScreenPoint(worldPoint);
            return IsInScreen(screenPoint);
        }

        public static bool IsInScreen(Vector2 screenPoint)
        {
            if (screenPoint.x < 0f || screenPoint.x > Screen.width || screenPoint.y < 0f || screenPoint.y > Screen.height)
            {
                return false;
            }

            return true;
        }
    }

    public enum AnchorLeftType
    {
        Top,
        Middle,
        Bottom,
        Stretch
    }

    public enum AnchorTopType
    {
        Left,
        Center,
        Right,
        Stretch
    }
}