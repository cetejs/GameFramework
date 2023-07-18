using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameFramework
{
    internal class DevCmdNode : IComparable<DevCmdNode>, IEnumerable<DevCmdNode>
    {
        private string name;
        private int order;
        private object[] args;
        private DevCmdNode parent;
        private MethodInfo info;
        private List<DevCmdNode> children;
        private string fixName;

        public DevCmdNode(string name, int order, DevCmdNode parent, MethodInfo info, object args = null)
        {
            this.name = name;
            this.order = order;
            this.parent = parent;
            this.info = info;
            fixName = name;

            if (args != null)
            {
                this.args = new[] {args};
            }
        }

        public string Name
        {
            get { return fixName; }
        }

        public object[] Args
        {
            get { return args; }
        }

        public string FullName
        {
            get { return parent == null ? name : string.Concat(parent.FullName, "/", fixName); }
        }

        public DevCmdNode Parent
        {
            get { return parent; }
        }

        public MethodInfo Info
        {
            get { return info; }
        }

        public bool IsRoot
        {
            get { return parent == null; }
        }

        public bool HasChild
        {
            get { return children != null && children.Count > 0; }
        }

        public DevCmdNode GetOrAddChild(string name, int order, MethodInfo info, object args = null)
        {
            if (children == null)
            {
                children = new List<DevCmdNode>();
            }
            else
            {
                foreach (DevCmdNode child in children)
                {
                    if (child.name == name)
                    {
                        return child;
                    }
                }
            }

            this.order = Math.Min(this.order, order);
            DevCmdNode node = new DevCmdNode(name, order, this, info, args);
            children.Add(node);
            return node;
        }

        public void FixName()
        {
            if (HasChild)
            {
                return;
            }

            ParameterInfo[] parameters = info.GetParameters();
            if (args == null && parameters.Length > 0)
            {
                fixName = string.Concat(name, "(");
                string step = "";
                foreach (ParameterInfo p in parameters)
                {
                    fixName = string.Concat(fixName, step, p.ParameterType.Name);
                    step = ", ";
                }

                fixName = string.Concat(fixName, ")");
            }
        }

        public void Sort()
        {
            if (!HasChild)
            {
                return;
            }

            children.Sort();
            foreach (DevCmdNode child in children)
            {
                child.Sort();
            }
        }

        public int CompareTo(DevCmdNode other)
        {
            return order.CompareTo(other.order);
        }

        public IEnumerator<DevCmdNode> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}