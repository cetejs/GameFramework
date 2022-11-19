using UnityEngine;

namespace GameFramework.Utils
{
    public static class VectorUtils
    {
        public static Vector3 SetX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }

        public static Vector3 SetXZ(this Vector3 v, float x, float z)
        {
            v.x = x;
            v.z = z;
            return v;
        }

        public static float SqrDistance(Vector3 a, Vector3 b, bool isIgnoreY)
        {
            if (isIgnoreY)
            {
                a.y = b.y = 0.0f;
            }

            return (a - b).sqrMagnitude;
        }

        public static bool Approximately(Vector3 a, Vector3 b, float diff = 0.1f)
        {
            if (Mathf.Abs(a.x - b.x) > diff)
            {
                return false;
            }
            
            if (Mathf.Abs(a.y - b.y) > diff)
            {
                return false;
            }
            
            if (Mathf.Abs(a.z - b.z) > diff)
            {
                return false;
            }

            return true;
        }
    }
}