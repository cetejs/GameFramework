using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumConditionAttribute : PropertyAttribute
    {
        public readonly string Condition;
        public readonly bool Hidden;
        public readonly int Bit;

        public bool Contains(int value)
        {
            return ((Bit >> value) & 1) == 1;  
        }

        public EnumConditionAttribute(string condition, params int[] values)
        {
            Condition = condition;
            Hidden = true;
            foreach (int value in values) 
            {
                Bit |= 1 << value;
            }
        }

        public EnumConditionAttribute(string condition, bool hideInInspector, params int[] values)
        {
            Condition = condition;
            Hidden = hideInInspector;
        }
    }
}