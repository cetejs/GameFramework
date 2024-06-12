using System;

namespace GameFramework
{
    public partial class DataNodeManager : PersistentSingleton<DataNodeManager>
    {
        private static readonly string[] EmptyStringArray = new string[] { };
        private static readonly string[] PathSplitSeparator = new string[] {".", "/", "\\"};

        private const string RootName = "<Root>";
        private IDataNode root;

        protected override void Awake()
        {
            base.Awake();
            root = DataNode.Create(RootName, null);
        }

        public IDataNode Root
        {
            get { return root; }
        }

        public T GetData<T>(string path, IDataNode node = null) where T : GameData
        {
            IDataNode current = GetNode(path, node);
            if (current == null)
            {
                GameLogger.LogError($"Data node is not exist, path {path}, node {(node != null ? node.FullName : string.Empty)}");
                return default;
            }

            return current.GetData<T>();
        }

        public void SetData<T>(string path, T data, IDataNode node = null) where T : GameData
        {
            IDataNode current = GetOrAddNode(path, node);
            current.SetData(data);
        }

        public void SetData(string path, GameData gameData, IDataNode node = null)
        {
            IDataNode current = GetOrAddNode(path, node);
            current.SetData(gameData);
        }

        public bool HasNode(string path, IDataNode node = null)
        {
            return GetNode(path, node) != null;
        }

        public IDataNode GetNode(string path, IDataNode node = null)
        {
            IDataNode current = node ?? root;
            string[] splitNames = GetSplitPath(path);
            foreach (string n in splitNames)
            {
                current = current.GetChild(n);
                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        public IDataNode GetOrAddNode(string path, IDataNode node = null)
        {
            IDataNode current = node ?? root;
            string[] splitNames = GetSplitPath(path);
            foreach (string n in splitNames)
            {
                current = current.GetOrAddChild(n);
            }

            return current;
        }

        public void RemoveNode(string path, IDataNode node = null)
        {
            IDataNode current = node ?? root;
            IDataNode parent = current.Parent;
            string[] splitNames = GetSplitPath(path);
            foreach (string n in splitNames)
            {
                parent = current;
                current = current.GetChild(n);
                if (current == null)
                {
                    return;
                }
            }

            if (parent != null)
            {
                parent.RemoveChild(current.Name);
            }
        }

        public void Clear()
        {
            root.Clear();
        }

        private static string[] GetSplitPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return EmptyStringArray;
            }

            return path.Split(PathSplitSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}