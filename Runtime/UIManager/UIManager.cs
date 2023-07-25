using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        private Transform windowRoot;
        private readonly Dictionary<string, UIWindow> prefabs = new Dictionary<string, UIWindow>();
        private readonly Dictionary<string, UIWindow> allWindows = new Dictionary<string, UIWindow>();
        private readonly Dictionary<string, UIWindow> showWindows = new Dictionary<string, UIWindow>();
        private readonly Dictionary<int, Transform> allLayers = new Dictionary<int, Transform>();
        private readonly List<UIWindow> fullScreenWindows = new List<UIWindow>();
        private readonly List<string> toRemoveWindows = new List<string>();
        private readonly HashSet<string> loadingWindows = new HashSet<string>();
        private readonly Heap<UIWindow> heapWindows = new Heap<UIWindow>();

        public Transform WindowRoot
        {
            get { return windowRoot; }
        }

        public void ShowWindow(string windowName, GameData data = null)
        {
            if (showWindows.ContainsKey(windowName))
            {
                GameLogger.LogError($"Window {windowName} is already show");
                return;
            }

            if (loadingWindows.Contains(windowName))
            {
                GameLogger.LogError($"Window {windowName} is loading");
                return;
            }

            if (!allWindows.TryGetValue(windowName, out UIWindow window))
            {
                if (!prefabs.TryGetValue(windowName, out UIWindow prefab))
                {
                    string windowPath = PathUtils.Combine(GameSettings.Instance.WindowAssetName, windowName);
                    GameObject go = AssetManager.Instance.LoadAsset<GameObject>(windowPath);
                    if (go == null)
                    {
                        GameLogger.LogError($"Window {windowName} is not exist");
                        return;
                    }

                    prefab = go.GetComponent<UIWindow>();
                    prefabs.Add(windowName, prefab);
                }

                window = InternalCreateWindow(prefab);
                InternalShowWindow(window, windowName, data);
                allWindows.Add(windowName, window);
            }
            else
            {
                InternalShowWindow(window, windowName, data);
            }
        }

        public void ShowWindowAsync(string windowName, Action callback)
        {
            ShowWindowAsync(windowName, null, callback);
        }

        public void ShowWindowAsync(string windowName, GameData data = null, Action callback = null)
        {
            if (showWindows.ContainsKey(windowName))
            {
                GameLogger.LogError($"Window {windowName} is already show");
                return;
            }

            if (loadingWindows.Contains(windowName))
            {
                GameLogger.LogError($"Window {windowName} is loading");
                return;
            }

            if (!allWindows.TryGetValue(windowName, out UIWindow window))
            {
                if (!prefabs.TryGetValue(windowName, out UIWindow prefab))
                {
                    string windowPath = PathUtils.Combine(GameSettings.Instance.WindowAssetName, windowName);
                    AssetAsyncOperation operation = AssetManager.Instance.LoadAssetAsync(windowPath);
                    loadingWindows.Add(windowName);
                    operation.OnCompleted += _ =>
                    {
                        GameObject go = operation.GetResult<GameObject>();
                        if (go == null)
                        {
                            GameLogger.LogError($"Window {windowName} is not exist");
                            loadingWindows.Remove(windowName);
                            return;
                        }

                        prefab = go.GetComponent<UIWindow>();
                        prefabs.Add(windowName, prefab);
                        window = InternalCreateWindow(prefab);
                        InternalShowWindow(window, windowName, data);
                        allWindows.Add(windowName, window);
                        loadingWindows.Remove(windowName);
                        callback?.Invoke();
                    };
                }
                else
                {
                    window = InternalCreateWindow(prefab);
                    InternalShowWindow(window, windowName, data);
                    allWindows.Add(windowName, window);
                    callback?.Invoke();
                }
            }
            else
            {
                InternalShowWindow(window, windowName, data);
                callback?.Invoke();
            }
        }

        public void HideWindow(string windowName)
        {
            if (showWindows.TryGetValue(windowName, out UIWindow window))
            {
                window.gameObject.SetActive(false);
                window.HideWindow();
                PopFullWindow(window, false);
                showWindows.Remove(windowName);
                SelectedWindow();
            }
            else
            {
                GameLogger.LogError($"Window {windowName} is not exist in show windows");
            }
        }

        public void CloseWindow(string windowName)
        {
            if (showWindows.TryGetValue(windowName, out UIWindow window))
            {
                window.CloseWindow();
                ReferencePool.Instance.Release(window.GetData<GameData>());
                PopFullWindow(window, false);
                Destroy(window.gameObject);
                showWindows.Remove(windowName);
                allWindows.Remove(windowName);
                SelectedWindow();
            }
            else
            {
                GameLogger.LogError($"Window {windowName} is not exist in show windows");
            }
        }

        public void CloseAllWindow()
        {
            foreach (UIWindow window in allWindows.Values)
            {
                window.CloseWindow();
                GameData data = window.GetData<GameData>();
                ReferencePool.Instance.Release(data);
                Destroy(window.gameObject);
            }

            allWindows.Clear();
            showWindows.Clear();
            fullScreenWindows.Clear();
        }

        public void CloseAllWindowExcluding(params string[] excluding)
        {
            if (excluding.Length == 0)
            {
                CloseAllWindow();
                return;
            }

            toRemoveWindows.Clear();
            foreach (UIWindow window in allWindows.Values)
            {
                if (excluding.Contains(window.WindowName))
                {
                    continue;
                }

                toRemoveWindows.Add(window.WindowName);
            }

            for (int i = toRemoveWindows.Count - 1; i >= 0; i--)
            {
                CloseWindow(toRemoveWindows[i]);
            }
        }

        public bool HasWindow(string windowName)
        {
            return showWindows.ContainsKey(windowName);
        }

        public T GetWindow<T>(string windowName) where T : UIWindow
        {
            allWindows.TryGetValue(windowName, out UIWindow window);
            return window as T;
        }

        protected override void Awake()
        {
            base.Awake();
            BuildWindowLayers();
        }

        private void BuildWindowLayers()
        {
            GameSettings setting = GameSettings.Instance;
            if (!string.IsNullOrEmpty(setting.WindowRootName))
            {
                string rooPath = PathUtils.Combine(GameSettings.Instance.WindowAssetName, setting.WindowRootName);
                GameObject go = AssetManager.Instance.LoadAsset<GameObject>(rooPath);
                GameObject result = Instantiate(go, transform);
                windowRoot = result.transform;
#if UNITY_EDITOR
                result.name = result.name.Replace("(Clone)", "");
#endif
                for (int i = 0; i < setting.WindowLayers.Length; i++)
                {
                    AddWindowLayer(i, setting.WindowLayers[i]);
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
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                windowRoot.SetParent(transform);
                for (int i = 0; i < setting.WindowLayers.Length; i++)
                {
                    AddWindowLayer(i, setting.WindowLayers[i]);
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

        private UIWindow InternalCreateWindow(UIWindow prefab)
        {
            Transform parent = allLayers[prefab.Layer];
            UIWindow window = Instantiate(prefab, parent);
#if UNITY_EDITOR
            window.name = window.name.Replace("(Clone)", "");
#endif
            window.transform.ResetLocal();
            SetSiblingIndex(window);
            window.CreateWindow();
            return window;
        }

        private void InternalShowWindow(UIWindow window, string windowName, GameData data)
        {
            window.gameObject.SetActive(true);
            window.InitData(windowName, data);
            window.ShowWindow();
            PopFullWindow(window, true);
            showWindows.Add(windowName, window);
            SelectedWindow();
        }

        private void PopFullWindow(UIWindow window, bool isShow)
        {
            if (window.Layer > 0)
            {
                return;
            }

            if (isShow)
            {
                window.gameObject.SetActive(false);
                if (fullScreenWindows.Count > 0)
                {
                    fullScreenWindows[0].gameObject.SetActive(false);
                }

                fullScreenWindows.Add(window);
                fullScreenWindows.Sort((a, b) => -a.Depth.CompareTo(b.Depth));
                fullScreenWindows[0].gameObject.SetActive(true);
            }
            else
            {
                int index = fullScreenWindows.IndexOf(window);
                fullScreenWindows.RemoveAt(index);
                if (index == 0 && fullScreenWindows.Count > 0)
                {
                    fullScreenWindows[0].gameObject.SetActive(true);
                }
            }
        }

        private void SelectedWindow()
        {
            if (showWindows.Count == 0)
            {
                return;
            }

            heapWindows.Clear();
            heapWindows.AddRange(showWindows.Values);
            InputManager.Instance.SetSelectedGameObject(heapWindows.Max.DefaultSelectedGo);
        }
    }
}