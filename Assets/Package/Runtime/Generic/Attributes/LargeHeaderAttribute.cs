using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LargeHeaderAttribute : PropertyAttribute
    {
        public readonly string name;
        public readonly string color;
        public readonly float alpha;
        public readonly int size;

        public LargeHeaderAttribute(string name, string color = "#FFFFFF", float alpha = 0.7f, int size = 16)
        {
            this.name = name;
            this.color = color;
            this.alpha = alpha;
            this.size = size;
        }
    }
}