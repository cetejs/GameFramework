using UnityEngine;

namespace GameFramework
{
    public class FlagAttribute : PropertyAttribute
    {
        public readonly string name;

        public FlagAttribute()
        {
        }

        public FlagAttribute(string name)
        {
            this.name = name;
        }
    }
}