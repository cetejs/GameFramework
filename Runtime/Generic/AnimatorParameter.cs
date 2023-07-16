using UnityEngine;

namespace GameFramework
{
    public struct AnimatorParameter
    {
        private AnimatorControllerParameter parameter;

        public readonly bool IsValid;

        public AnimatorParameter(Animator anim, string param)
        {
            if (anim && TryGetParam(anim, param, out AnimatorControllerParameter p))
            {
                parameter = p;
                IsValid = true;
            }
            else
            {
                parameter = null;
                IsValid = false;
            }
        }

        public static implicit operator int(AnimatorParameter a)
        {
            if (a.IsValid)
            {
                return a.parameter.nameHash;
            }

            return -1;
        }

        private static bool TryGetParam(Animator anim, string param, out AnimatorControllerParameter result)
        {
            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == param)
                {
                    result = p;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}