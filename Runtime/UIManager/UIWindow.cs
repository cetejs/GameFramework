using UnityEngine;

namespace GameFramework
{
    public abstract class UIWindow : MonoBehaviour
    {
        [UIWindowLayer]
        [SerializeField]
        private int layer;
        [SerializeField]
        private int depth;

        private string windowName;
        private GameData data;

        public int Layer
        {
            get { return layer; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public string WindowName
        {
            get { return windowName; }
        }

        public T GetData<T>() where T : GameData
        {
            return data as T;
        }

        public void Hide()
        {
            UIManager.Instance.HideWindow(windowName);
        }

        public void Close()
        {
            UIManager.Instance.CloseWindow(windowName);
        }

        internal void InitData(string windowName, GameData data)
        {
            this.windowName = windowName;
            this.data = data;
            OnInitData();
        }

        private void Update()
        {
            OnUpdateWindow();
        }

        internal void CreateWindow()
        {
            OnCreateWindow();
        }

        internal void CloseWindow()
        {
            OnCloseWindow();
        }

        internal void ShowWindow()
        {
            OnShowWindow();
        }

        internal void HideWindow()
        {
            OnHideWindow();
        }

        protected virtual void OnInitData()
        {
        }

        protected virtual void OnCreateWindow()
        {
        }

        protected virtual void OnCloseWindow()
        {
        }

        protected virtual void OnShowWindow()
        {
        }

        protected virtual void OnHideWindow()
        {
        }

        protected virtual void OnUpdateWindow()
        {
        }
    }
}