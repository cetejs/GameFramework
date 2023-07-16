using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReplaceNameAttribute : PropertyAttribute
    {
        public readonly string Name;

        public ReplaceNameAttribute(string name)
        {
            Name = name;
        }
    }
}