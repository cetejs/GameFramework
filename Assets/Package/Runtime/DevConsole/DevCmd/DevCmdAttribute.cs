using System;

namespace GameFramework.DevConsoleService
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DevCmdAttribute : Attribute
    {
        public DevCmdAttribute(string name, int order = int.MaxValue, params object[] args)
        {
            Name = name;
            Order = order;
            Args = args;
        }

        public string Name { get; private set; }

        public int Order { get; private set; }

        public object[] Args { get; private set; }
    }
}