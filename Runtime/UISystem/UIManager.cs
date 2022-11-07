using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Generic;
using GameFramework.InputService;
using GameFramework.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace GameFramework.UIService
{
    public class UIManager : PersistentService
    {
        private Transform windowRoot;
        private readonly Dictionary<string, UIWindow> allWindows = new Dictionary<string, UIWindow>();
        private readonly Dictionary<string, UIWindow> showWindows = new Dictionary<string, UIWindow>();
        private readonly Dictionary<int, Transform> allLayers = new Dictionary<int, Transform>();
        private readonly List<UIWindow> fullScreenWindows = new List<UIWindow>();
        private readonly HashSet<string> loadingWindows = new HashSet<string>();

        public Transform WindowRoot
        {
            get { return windowRoot; }
        }

        protected override void Awake()
        {
            base.Awake();
            BuildWindowLayers();
        }

        private void Start()
        {
            Global.RequireService<InputManager>();
        }

        public async void ShowWindow(string windowName, Data data = null)
        {
            if (showWindows.ContainsKey(windowName))
            {
                Debug.LogError($"Window {windowName} is already show");
                return;
            }

            if (loadingWindows.Contains(windowName))
            {
                Debug.LogError($"Window {windowName} is loading");
                return;
            }

            if (!allWindows.TryGetValue(windowName, out UIWindow window))
            {
                loadingWindows.Add(windowName);
                string windowPath = String.Concat(UIConfig.Get().windowBundlePath, "/", windowName, ".prefab");
                GameObject result = await Addressables.InstantiateAsync(windowPath).Task;
                loadingWindows.Remove(windowName);
#if UNITY_EDITOR
                result.name = result.name.Replace("(Clone)", "");
#endif
                window = result.GetComponent<UIWindow>();
                Transform parent = allLayers[window.Layer];
                window.transform.SetParent(parent, false);
                window.transform.ResetLocal();
                SetSiblingIndex(window);
                allWindows.Add(windowName, window);
                window.InitData(windowName, data);
                window.OnCreatWindow();
                window.OnShowWindow();
                HandleFullWindow(window, true);
                showWindows.Add(windowName, window);
            }
            else
            {
                window.gameObject.SetActive(true);
                window.InitData(windowName, data);
                window.OnShowWindow();
                HandleFullWindow(window, true);
                showWindows.Add(windowName, window);
            }
        }

        public void HideWindow(string windowName)
        {
            if (showWindows.TryGetValue(windowName, out UIWindow window))
            {
                window.gameObject.SetActive(false);
                window.OnHideWindow();
                showWindows.Remove(windowName);
                HandleFullWindow(window, false);
            }
            else
            {
                Debug.LogError($"Window {windowName} is not exist in show windows");
            }
        }

        public void CloseWindow(string windowName)
        {
            if (showWindows.TryGetValue(windowName, out UIWindow window))
            {
                showWindows.Remove(windowName);
                allWindows.Remove(windowName);
                window.OnCloseWindow();
                HandleFullWindow(window, false);
                ReferencePool.Release(window.GetData());
                Addressables.ReleaseInstance(window.gameObject);
            }
            else
            {
                Debug.LogError($"Window {windowName} is not exist in show windows");
            }
        }

        public void CloseAllWindow()
        {
            foreach (UIWindow window in allWindows.Values)
            {
                window.OnCloseWindow();
                Data data = window.GetData();
                ReferencePool.Release(data);
                Addressables.ReleaseInstance(window.gameObject);
            }

            allWindows.Clear();
            showWindows.Clear();
            fullScreenWindows.Clear();
        }

        public void CloseAllWindowExcluding(params string[] excluding)
        {
            foreach (UIWindow window in allWindows.Values)
            {
                if (excluding.Contains(window.WindowName))
                {
                    continue;
                }

                window.OnCloseWindow();
                Data data = window.GetData();
                ReferencePool.Release(data);
                Addressables.ReleaseInstance(window.gameObject);
            }

            allWindows.Clear();
            showWindows.Clear();
            fullScreenWindows.Clear();
        }

        public bool HasWindow(string windowName)
        {
            return showWindows.ContainsKey(windowName);
        }

        public UIWindow GetWindow(string windowName)
        {
            allWindows.TryGetValue(windowName, out UIWindow window);
            return window;
        }

        private async void BuildWindowLayers()
        {
            UIConfig config = UIConfig.Get();
            if (config.windowRoot.RuntimeKeyIsValid())
            {
                GameObject result = await Addressables.InstantiateAsync(config.windowRoot).Task;
                windowRoot = result.transform;
                windowRoot.SetParent(transform);
#if UNITY_EDITOR
                result.name = result.name.Replace("(Clone)", "");
#endif
                for (int i = 0; i < config.windowLayers.Length; i++)
                {
                    AddWindowLayer(i, config.windowLayers[i]);
                }
            }
            else
            {
                windowRoot = new GameObject("WindowRoot").AddComponent<RectTransform>();
                Canvas canvas = windowRoot.gameObject.AddComponent<Canvas>();
                CanvasScaler scaler = windowRoot.gameObject.AddComponent<CanvasScaler>();
                windowRoot.gameObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
                windowRoot.SetParent(transform);
                for (int i = 0; i < config.windowLayers.Length; i++)
                {
                    AddWindowLayer(i, config.windowLayers[i]);
                }
            }
        }

        private void AddWindowLayer(int layer, string layerName)
        {
            if (!windowRoot.Find(layerName))
            {
                RectTransform rectTrs = new GameObject(layerName).AddComponent<RectTransform>();
                rectTrs.SetParent(windowRoot);
                rectTrs.AdjustAnchor(AnchorLeftType.Stretch, AnchorTopType.Stretch);
                rectTrs.ResetLocal();
                allLayers.Add(layer, rectTrs);
            }
        }

        private void SetSiblingIndex(UIWindow window)
        {
            int index = 0;
            foreach (UIWindow tempWindow in allWindows.Values)
            {
                if (tempWindow.Layer != window.Layer)
                {
                    continue;
                }

                if (tempWindow.Depth > window.Depth)
                {
                    continue;
                }

                int tempIndex = tempWindow.transform.GetSiblingIndex();
                index = Mathf.Max(index, tempIndex + 1);
            }

            window.transform.SetSiblingIndex(index);
        }

        private void HandleFullWindow(UIWindow window, bool isShow)
        {
            if (window.Layer > 0)
            {
                return;
            }

            if (isShow)
            {
                window.gameObject.SetActiveEx(false);
                if (fullScreenWindows.Count > 0)
                {
                    fullScreenWindows[0].gameObject.SetActiveEx(false);
                }

                fullScreenWindows.Add(window);
                fullScreenWindows.Sort((a, b) => -a.Depth.CompareTo(b.Depth));
                fullScreenWindows[0].gameObject.SetActiveEx(true);
            }
            else
            {
                int index = fullScreenWindows.IndexOf(window);
                fullScreenWindows.RemoveAt(index);
                if (index == 0 && fullScreenWindows.Count > 0)
                {
                    fullScreenWindows[0].gameObject.SetActiveEx(true);
                }
            }
        }
    }
}