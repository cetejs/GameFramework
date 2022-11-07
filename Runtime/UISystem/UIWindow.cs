using GameFramework.Generic;
using UnityEngine;

namespace GameFramework.UIService
{
    public abstract class UIWindow : MonoBehaviour
    {
        [SerializeField]
        private int layer;
        [SerializeField]
        private int depth;

        private string windowName;
        private Data data;

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

        private void Update()
        {
            OnUpdateWindow();
        }

        public void Hide()
        {
            Global.GetService<UIManager>().HideWindow(windowName);
        }

        public void Close()
        {
            Global.GetService<UIManager>().CloseWindow(windowName);
        }

        public Data GetData()
        {
            return data;
        }

        public T GetData<T>() where T : Data
        {
            return data as T;
        }

        public void InitData(string windowName, Data data)
        {
            this.windowName = windowName;
            this.data = data;
            OnInitData();
        }

        public virtual void OnInitData()
        {
        }

        public virtual void OnCreatWindow()
        {
        }

        public virtual void OnCloseWindow()
        {
        }

        public virtual void OnShowWindow()
        {
        }

        public virtual void OnHideWindow()
        {
        }

        public virtual void OnUpdateWindow()
        {
        }
    }
}