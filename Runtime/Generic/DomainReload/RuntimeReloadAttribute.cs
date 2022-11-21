using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RuntimeReloadAttribute : Attribute
    {
        public readonly object value;

        public RuntimeReloadAttribute(object value)
        {
            this.value = value;
        }
    }
}