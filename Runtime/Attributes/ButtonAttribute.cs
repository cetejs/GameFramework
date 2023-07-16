using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public ButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}