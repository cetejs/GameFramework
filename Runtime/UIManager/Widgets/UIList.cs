using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIList : MonoBehaviour
    {
        [SerializeField]
        private Direction direction = Direction.Horizontal;
        [SerializeField]
        private Padding padding;
        [SerializeField]
        private float spacing;
        [SerializeField]
        private int row = 1;
        [SerializeField]
        private Vector2 cellSize;
        [SerializeField] [ObjectPoolName]
        private string poolName;

        private ScrollRect scrollRect;
        private RectTransform viewRect;
        private RectTransform content;
        private ObjectPool<UICell> pool;
        private Action<UICell> onCellUpdate;
        private bool isAdjustAnchored;
        private readonly List<CellInfo> list = new List<CellInfo>();

        public int Count
        {
            get { return list.Count; }
        }

        public RectTransform Content
        {
            get { return content; }
        }

        public event Action<UICell> OnCellUpdate
        {
            add { onCellUpdate += value; }
            remove { onCellUpdate -= value; }
        }

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewRect = GetComponent<RectTransform>();
            content = scrollRect.content;
            scrollRect.onValueChanged.AddListener(OnScrollViewChanged);
            scrollRect.horizontal = direction == Direction.Horizontal;
            scrollRect.vertical = direction == Direction.Vertical;
        }

        private void Start()
        {
            if (pool == null && !string.IsNullOrEmpty(poolName))
            {
                ObjectPool objPool = ObjectPoolManager.Instance.GetObjectPool(poolName);
                pool = new ObjectPool<UICell>(objPool);
                if (cellSize == Vector2.zero)
                {
                    cellSize = pool.Get(objPool.transform).RectTransform.sizeDelta;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (CellInfo cellInfo in list)
            {
                if (cellInfo.cell != null)
                {
                    cellInfo.cell.Release();
                }
            }

            list.Clear();
        }

        public void Init(UICell cell)
        {
            if (!cell)
            {
                GameLogger.LogError("Init failed, because cell is invalid");
                return;
            }

            pool = new ObjectPool<UICell>(transform);
            pool.Init(cell);
            if (cellSize == Vector2.zero)
            {
                cellSize = cell.RectTransform.sizeDelta;
            }
        }

        public void SetCount(int count)
        {
            if (count < 0)
            {
                GameLogger.LogError("Set cell count is invalid");
                return;
            }

            SetContentSize(count);
            SetListSize(count);
            UpdateView();
        }

        public void UpdateList()
        {
            for (int i = 0; i < list.Count; i++)
            {
                CellUpdate(i);
            }
        }

        public void CellUpdate(int index)
        {
            CellInfo cellInfo = list[index];
            if (cellInfo.cell == null)
            {
                return;
            }

            if (IsOutRange(cellInfo.pos))
            {
                return;
            }

            onCellUpdate?.Invoke(cellInfo.cell);
        }

        private void SetContentSize(int count)
        {
            if (direction == Direction.Horizontal)
            {
                float contentWight = (spacing + cellSize.x) * Mathf.CeilToInt(count / (float) row);
                contentWight -= spacing;
                contentWight += padding.left + padding.right;
                Vector2 contentPos = content.anchoredPosition;
                Vector2 contentSize = content.sizeDelta;
                contentSize.x = contentWight;
                contentPos.x = Mathf.Min(contentWight - viewRect.sizeDelta.x, contentPos.x);
                contentPos.x = Mathf.Max(contentPos.x, 0);
                content.anchoredPosition = contentPos;
                content.sizeDelta = contentSize;
                scrollRect.horizontal = contentSize.x > viewRect.sizeDelta.x;
                if (!isAdjustAnchored)
                {
                    isAdjustAnchored = true;
                    content.AdjustAnchor(AnchorLeftType.Stretch, AnchorTopType.Left);
                }
            }
            else
            {
                float contentHeight = (spacing + cellSize.y) * Mathf.CeilToInt(count / (float) row);
                contentHeight -= spacing;
                contentHeight += padding.top + padding.bottom;
                Vector2 contentPos = content.anchoredPosition;
                Vector2 contentSize = content.sizeDelta;
                contentSize.y = contentHeight;
                contentPos.y = Mathf.Min(contentHeight - viewRect.sizeDelta.y, contentPos.y);
                contentPos.y = Mathf.Max(contentPos.y, 0);
                content.anchoredPosition = contentPos;
                content.sizeDelta = contentSize;
                scrollRect.vertical = contentSize.y > viewRect.sizeDelta.y;
                if (!isAdjustAnchored)
                {
                    isAdjustAnchored = true;
                    content.AdjustAnchor(AnchorLeftType.Top, AnchorTopType.Stretch);
                }
            }
        }

        private void SetListSize(int count)
        {
            if (list.Count > count)
            {
                int diffCount = list.Count - count;
                while (diffCount > 0)
                {
                    int lastIndex = list.Count - 1;
                    UICell cell = list[lastIndex].cell;
                    if (cell)
                    {
                        cell.Release();
                    }

                    list.RemoveAt(lastIndex);
                    diffCount--;
                }
            }
            else
            {
                int diffCount = count - list.Count;
                while (diffCount > 0)
                {
                    int index = list.Count;
                    CellInfo cellInfo = new CellInfo {pos = GetCellPosition(index)};
                    list.Add(cellInfo);
                    diffCount--;
                }
            }
        }

        private void UpdateView()
        {
            for (int i = 0; i < list.Count; i++)
            {
                CellInfo cellInfo = list[i];
                if (IsOutRange(cellInfo.pos))
                {
                    if (cellInfo.cell)
                    {
                        cellInfo.cell.Release();
                        cellInfo.cell = null;
                    }
                }
                else
                {
                    if (cellInfo.cell == null)
                    {
                        UICell cell = pool.Get(content);
                        cell.Index = i;
                        cell.RectTransform.AdjustAnchor(AnchorLeftType.Top, AnchorTopType.Left);
                        cell.RectTransform.sizeDelta = cellSize;
                        cell.RectTransform.anchoredPosition = cellInfo.pos;
                        cell.RectTransform.localScale = Vector3.one;
                        cellInfo.cell = cell;
                        onCellUpdate?.Invoke(cell);
                    }
                }

                list[i] = cellInfo;
            }
        }

        private Vector2 GetCellPosition(int index)
        {
            Vector2 pos = Vector2.zero;
            if (direction == Direction.Horizontal)
            {
                pos.x = (cellSize.x + spacing) * (index / row);
                pos.y = -(cellSize.y + spacing) * (index % row);
            }
            else
            {
                pos.x = (cellSize.x + spacing) * (index % row);
                pos.y = -(cellSize.y + spacing) * (index / row);
            }

            pos.x += padding.left;
            pos.y -= padding.top;
            return pos;
        }

        private bool IsOutRange(Vector2 pos)
        {
            Vector2 contentPos = content.anchoredPosition;
            Vector2 viewSize = viewRect.sizeDelta;
            if (direction == Direction.Horizontal)
            {
                float x = pos.x + contentPos.x;
                if (x < -cellSize.x || x > viewSize.x)
                {
                    return true;
                }
            }
            else
            {
                float y = pos.y + contentPos.y;
                if (y > cellSize.y || y < -viewSize.y)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnScrollViewChanged(Vector2 value)
        {
            UpdateView();
        }

        private enum Direction
        {
            Horizontal,
            Vertical
        }

        [Serializable]
        public struct Padding
        {
            public int left;
            public int right;
            public int top;
            public int bottom;
        }

        private struct CellInfo
        {
            public UICell cell;
            public Vector2 pos;
        }
    }
}