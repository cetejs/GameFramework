using System.Collections.Generic;
using GameFramework.Generic;
using GameFramework.ObjectPoolService;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.DevConsoleService
{
    internal class DevCmdGroup : PoolObject
    {
        [SerializeField]
        private float maxHeight = 600.0f;
        private ScrollRect scroll;
        private DevCmdHub hub;
        private DevCmdNode node;
        private RectTransform rectTrs;
        private readonly List<DevCmdItem> items = new List<DevCmdItem>();

        protected override void OnInit()
        {
            scroll = GetComponent<ScrollRect>();
            rectTrs = transform as RectTransform;
        }

        protected override void OnWakeUp()
        {
            Data<DevCmdHub, DevCmdNode, Vector2> info = GetData<Data<DevCmdHub, DevCmdNode, Vector2>>();
            hub = info.item1;
            node = info.item2;
            Vector3 pos = info.item3;

            if (items.Count > 0)
            {
                return;
            }

            foreach (DevCmdNode child in node)
            {
                AddChild(child);
            }

            float height = items.Count * hub.ItemSize.y;
            scroll.vertical = height > maxHeight;
            Vector2 size = rectTrs.sizeDelta;
            size.y = Mathf.Min(maxHeight, height);
            rectTrs.sizeDelta = size;
            rectTrs.position = pos;
        }

        protected override void OnSleep()
        {
            if (items.Count == 0)
            {
                return;
            }

            foreach (DevCmdItem item in items)
            {
                item.Release();
            }

            items.Clear();
        }

        private void AddChild(DevCmdNode child)
        {
            Data<DevCmdHub, DevCmdNode> info = ReferencePool.Get<Data<DevCmdHub, DevCmdNode>>();
            info.item1 = hub;
            info.item2 = child;
            DevCmdItem item = hub.ItemPool.Get(scroll.content, info);
            items.Add(item);
        }
    }
}