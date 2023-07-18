using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevCmdItem : PoolObject, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private Text cmdText;
        [SerializeField]
        private GameObject array;
        private DevCmdHub hub;
        private DevCmdNode node;
        private DevCmdGroup group;
        private RectTransform rectTrs;

        private static Action<DevCmdNode> onItemTrigger;

        public void SetData(DevCmdHub hub, DevCmdNode node)
        {
            this.hub = hub;
            this.node = node;
            bool hasArray = node.IsRoot || node.HasChild;
            cmdText.text = node.Name;
            array.SetActive(hasArray);
        }

        protected override void OnInit()
        {
            rectTrs = transform as RectTransform;
        }

        protected override void OnWakeUp()
        {
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
            if (node.IsRoot && group == null)
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
                    GameLogger.LogException(ex);
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
            if (group != null)
            {
                return;
            }

            group = hub.GroupPool.Get(hub.transform);
            group.SetData(hub, node, rectTrs.position);
        }

        public void HideGroup()
        {
            if (group == null)
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