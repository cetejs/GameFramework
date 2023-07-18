using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevCmdGroup : PoolObject
    {
        [SerializeField]
        private float maxHeight = 600f;
        private ScrollRect scroll;
        private DevCmdHub hub;
        private DevCmdNode node;
        private RectTransform rectTrs;
        private readonly List<DevCmdItem> items = new List<DevCmdItem>();

        public void SetData(DevCmdHub hub, DevCmdNode node, Vector3 pos)
        {
            this.hub = hub;
            this.node = node;
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

        protected override void OnInit()
        {
            scroll = GetComponent<ScrollRect>();
            rectTrs = transform as RectTransform;
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
            DevCmdItem item = hub.ItemPool.Get(scroll.content);
            item.SetData(hub, child);
            items.Add(item);
        }
    }
}