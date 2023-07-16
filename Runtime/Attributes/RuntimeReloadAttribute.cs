using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RuntimeReloadAttribute : Attribute
    {
        public readonly object Value;

        public RuntimeReloadAttribute()
        {
        }

        public RuntimeReloadAttribute(object value)
        {
            Value = value;
        }
    }
}