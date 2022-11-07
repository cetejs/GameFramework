using UnityEngine;

namespace GameFramework.Generic
{
    public struct AnimatorParameter
    {
        private readonly AnimatorControllerParameter parameter;

        public readonly bool isValid;

        public AnimatorParameter(Animator anim, string param)
        {
            if (anim && TryGetParam(anim, param, out AnimatorControllerParameter p))
            {
                parameter = p;
                isValid = true;
            }
            else
            {
                parameter = null;
                isValid = false;
            }
        }

        public static implicit operator int(AnimatorParameter a)
        {
            if (a.isValid)
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