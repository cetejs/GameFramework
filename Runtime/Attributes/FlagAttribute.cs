using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FlagAttribute : PropertyAttribute
    {
        public readonly string Name;

        public FlagAttribute()
        {
        }

        public FlagAttribute(string name)
        {
            Name = name;
        }
    }
}