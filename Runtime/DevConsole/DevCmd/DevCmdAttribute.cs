using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DevCmdAttribute : Attribute
    {
        public readonly string Name;

        public readonly int Order;

        public readonly object[] Args;

        public DevCmdAttribute(string name, int order = int.MaxValue, params object[] args)
        {
            Name = name;
            Order = order;
            Args = args;
        }
    }
}