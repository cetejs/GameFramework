using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevCmdHub : MonoBehaviour
    {
        [SerializeField]
        private DevCmdItem rootItem;
        [SerializeField]
        private InputField cmdInput;
        private DevCmdNode root;
        private ObjectPool<DevCmdGroup> cmdGroupPool;
        private ObjectPool<DevCmdItem> cmdItemPool;

        public Vector3 ItemSize { get; private set; }

        public string CmdText
        {
            get { return cmdInput.text; }
        }

        public ObjectPool<DevCmdGroup> GroupPool
        {
            get { return cmdGroupPool; }
        }

        public ObjectPool<DevCmdItem> ItemPool
        {
            get { return cmdItemPool; }
        }

#if ENABLE_CONSOLE
        private void Start()
        {
            root = new DevCmdNode("Command", -1, null, null);
            rootItem.Init(null);
            rootItem.WakeUp();
            rootItem.SetData(this, root);
            HashSet<string> assemblies = new HashSet<string>();
            assemblies.AddRange(GameSettings.Instance.GlobalAssemblyNames);
            assemblies.AddRange(GameSettings.Instance.DevConsoleAssemblyNames);

            foreach (string assembly in assemblies)
            {
                foreach (Type type in Assembly.Load(assembly).GetTypes())
                {
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (MethodInfo methodInfo in methodInfos)
                    {
                        if (!methodInfo.IsDefined(typeof(DevCmdAttribute)))
                        {
                            continue;
                        }

                        DevCmdAttribute attr = methodInfo.GetCustomAttribute<DevCmdAttribute>(true);
                        DevCmdNode current = root;
                        string[] splitNames = attr.Name.Split("/", StringSplitOptions.RemoveEmptyEntries);

                        foreach (string n in splitNames)
                        {
                            current = current.GetOrAddChild(n, attr.Order, methodInfo);
                        }

                        if (attr.Args != null && attr.Args.Length > 0)
                        {
                            foreach (object args in attr.Args)
                            {
                                current.GetOrAddChild(string.Concat(current.Name, ":", args), attr.Order, methodInfo, args);
                            }
                        }

                        current.FixName();
                    }
                }
            }

            DevCmdGroup groupPrefab = Resources.Load<DevCmdGroup>("DevCmdGroup");
            DevCmdItem itemPrefab = Resources.Load<DevCmdItem>("DevCmdItem");
            ItemSize = ((RectTransform) itemPrefab.transform).sizeDelta;

            root.Sort();
            cmdGroupPool = new ObjectPool<DevCmdGroup>(transform);
            cmdItemPool = new ObjectPool<DevCmdItem>(transform);
            cmdGroupPool.Init(groupPrefab, 5);
            cmdItemPool.Init(itemPrefab, 30);
        }

        private void OnEnable()
        {
            rootItem.HideGroup();
        }
#endif
    }
}