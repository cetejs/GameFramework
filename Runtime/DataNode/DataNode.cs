using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class DataNodeManager
    {
        private class DataNode : IDataNode
        {
            private static readonly IDataNode[] EmptyDataNodeArray = { };

            private string name;
            private GameData gameData;
            private IDataNode parent;
            private List<IDataNode> children;
            private Action<IDataNode> onValueChanged;

            public event Action<IDataNode> OnValueChanged
            {
                add
                {
                    if (gameData != null)
                    {
                        value?.Invoke(this);
                    }

                    onValueChanged += value;
                }
                remove { onValueChanged -= value; }
            }

            public static IDataNode Create(string name, IDataNode parent)
            {
                if (!IsValidName(name))
                {
                    GameLogger.LogError("Name of data node is invalid");
                }

                DataNode node = ReferencePool.Instance.Get<DataNode>();
                node.name = name;
                node.parent = parent;
                return node;
            }

            public string Name
            {
                get { return name; }
            }

            public string FullName
            {
                get { return parent == null ? name : string.Concat(parent.FullName, PathSplitSeparator[0], name); }
            }

            public IDataNode Parent
            {
                get { return parent; }
            }

            public int ChildCount
            {
                get { return children != null ? children.Count : 0; }
            }

            public T GetData<T>() where T : GameData
            {
                return (T) gameData;
            }

            public void SetData(GameData gameData)
            {
                ReferencePool.Instance.Release(this.gameData);
                this.gameData = gameData;
                onValueChanged?.Invoke(this);
            }

            public bool HasChild(int index)
            {
                return index >= 0 && index < ChildCount;
            }

            public bool HasChild(string name)
            {
                return GetChild(name) != null;
            }

            public IDataNode GetChild(int index)
            {
                if (HasChild(index))
                {
                    return children[index];
                }

                return null;
            }

            public IDataNode GetChild(string name)
            {
                if (!IsValidName(name))
                {
                    GameLogger.LogError("Name is invalid");
                }

                if (children == null)
                {
                    return null;
                }

                foreach (IDataNode child in children)
                {
                    if (child.Name == name)
                    {
                        return child;
                    }
                }

                return null;
            }

            public IDataNode GetOrAddChild(string name)
            {
                IDataNode node = GetChild(name);
                if (node != null)
                {
                    return node;
                }

                node = Create(name, this);

                if (children == null)
                {
                    children = new List<IDataNode>();
                }

                children.Add(node);

                return node;
            }

            public IDataNode[] GetAllChild()
            {
                if (children == null)
                {
                    return EmptyDataNodeArray;
                }

                return children.ToArray();
            }

            public void GetAllChild(List<IDataNode> results)
            {
                if (results == null)
                {
                    GameLogger.LogError("Results is invalid");
                    return;
                }

                results.Clear();
                if (children == null)
                {
                    return;
                }

                results.AddRange(children);
            }

            public void RemoveChild(int index)
            {
                IDataNode node = GetChild(index);
                if (node == null)
                {
                    return;
                }

                children.Remove(node);
                ReferencePool.Instance.Release(node);
            }

            public void RemoveChild(string name)
            {
                IDataNode node = GetChild(name);
                if (node == null)
                {
                    return;
                }

                children.Remove(node);
                ReferencePool.Instance.Release(node);
            }

            public void Clear()
            {
                name = null;
                parent = null;
                onValueChanged = null;
                if (gameData != null)
                {
                    ReferencePool.Instance.Release(gameData);
                    gameData = null;
                }

                if (children != null)
                {
                    foreach (IDataNode child in children)
                    {
                        ReferencePool.Instance.Release(child);
                    }

                    children.Clear();
                }
            }

            private string ToDataString()
            {
                if (gameData == null)
                {
                    return "<Null>";
                }

                return $"[{gameData.GetType().Name}] {gameData}";
            }

            public override string ToString()
            {
                return $"{FullName}: {ToDataString()}";
            }

            private static bool IsValidName(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return false;
                }

                foreach (string separator in PathSplitSeparator)
                {
                    if (name.Contains(separator))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}