using System;
using System.Reflection;
using GameFramework.Generic;
using GameFramework.ObjectPoolService;
using GameFramework.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.DevConsoleService
{
    internal class DevCmdItem : PoolObject, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private Text cmdText;
        [SerializeField]
        private GameObject array;
        [SerializeField]
        private bool isAllowEnter;
        private DevCmdHub hub;
        private DevCmdNode node;
        private DevCmdGroup group;
        private RectTransform rectTrs;

        private static Action<DevCmdNode> onItemTrigger;

        protected override void OnInit()
        {
            rectTrs = transform as RectTransform;
        }

        protected override void OnWakeUp()
        {
            Data<DevCmdHub, DevCmdNode> info = GetData<Data<DevCmdHub, DevCmdNode>>();
            hub = info.item1;
            node = info.item2;
            bool hasArray = node.IsRoot || node.HasChild;
            cmdText.text = node.Name;
            array.SetActiveEx(hasArray);
            onItemTrigger += OnItemTrigger;
        }

        protected override void OnSleep()
        {
            onItemTrigger -= OnItemTrigger;
            HideGroup();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
            if (!isAllowEnter)
            {
                return;
            }

            onItemTrigger?.Invoke(node);
            if (!node.HasChild)
            {
                return;
            }

            ShowGroup();
#endif
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onItemTrigger?.Invoke(node);
            if (node.IsRoot && !node.HasChild)
            {
                return;
            }

            if (!node.HasChild)
            {
                try
                {
                    ParameterInfo[] parameters = node.Info.GetParameters();
                    if (parameters.Length == 0)
                    {
                        node.Info.Invoke(null, null);
                    }
                    else if (parameters.Length == 1 && node.Args != null)
                    {
                        node.Info.Invoke(null, node.Args);
                    }
                    else
                    {
                        string[] argsText = hub.CmdText.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        object[] args = new object[parameters.Length];
                        for (int i = 0; i < args.Length && i < argsText.Length; i++)
                        {
                            args[i] = JsonUtils.ConvertToObject(argsText[i], parameters[i].ParameterType);
                        }

                        node.Info.Invoke(null, args);
                    }
                }
                catch (Exception ex)
                {
                    GameLogger.LogError($"DevCmdItem is thrown exception : {ex} {this}");
                }
            }
            else
            {
                if (!group)
                {
                    ShowGroup();
                }
                else
                {
                    HideGroup();
                }
            }
        }

        public void ShowGroup()
        {
            if (group)
            {
                return;
            }

            Data<DevCmdHub, DevCmdNode, Vector2> info = ReferencePool.Get<Data<DevCmdHub, DevCmdNode, Vector2>>();
            info.item1 = hub;
            info.item2 = node;
            info.item3 = rectTrs.position;
            group = hub.GroupPool.Get(hub.transform, info);
        }

        public void HideGroup()
        {
            if (!group)
            {
                return;
            }

            group.Release();
            group = null;
        }

        private void OnItemTrigger(DevCmdNode tempNode)
        {
            if (tempNode.FullName.StartsWith(node.FullName))
            {
                return;
            }

            HideGroup();
        }
    }
}