using System;
using System.Reflection;
using GameFramework.Generic;
using GameFramework.ObjectPoolService;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.DevConsoleService
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

#if UNITY_EDITOR || ENABLE_CONSOLE
        private void Start()
        {
            root = new DevCmdNode("Command", -1, null, null);
            Data<DevCmdHub, DevCmdNode> info = ReferencePool.Get<Data<DevCmdHub, DevCmdNode>>();
            info.item1 = this;
            info.item2 = root;
            rootItem.Init(null);
            rootItem.WakeUp(info);

            DevConsoleConfig config = DevConsoleConfig.Get();
            foreach (string assembly in config.cmdAssemblies)
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
            cmdGroupPool = new ObjectPool<DevCmdGroup>("DevCmdGroup");
            cmdItemPool = new ObjectPool<DevCmdItem>("DevCmdItem");
            cmdGroupPool.Init(transform, groupPrefab, 5);
            cmdItemPool.Init(transform, itemPrefab, 30);
        }

        private void OnEnable()
        {
            rootItem.HideGroup();
        }
#endif
    }
}