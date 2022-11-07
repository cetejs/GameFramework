using UnityEngine;

namespace GameFramework.Utils
{
    public static class MathUtils
    {
        public static bool IsBetween(this int v, int a, int b)
        {
            return v >= a && v <= b;
        }

        public static bool IsBetween(this float v, float a, float b)
        {
            return v >= a && v <= b;
        }

        public static float WrapPI(float a)
        {
            if (Mathf.Abs(a) > 180.0f)
            {
                return a - 360.0f * Mathf.Floor((a + 180.0f) / 360.0f);
            }

            return a;
        }

        public static float DiffAngle(float a, float b)
        {
            float diff = (a - b) % 360.0f;
            if (diff > 180.0f)
            {
                diff = 360.0f - diff;
            }
            else if (diff < -180.0f)
            {
                diff = 360.0f + diff;
            }

            return diff;
        }
    }
}