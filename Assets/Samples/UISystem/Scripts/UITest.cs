using System.IO;
using GameFramework.Generic;
using GameFramework.UIService;
using GameFramework.Utils;
using UnityEngine;

namespace GameFramework
{
    public class UITest : MonoBehaviour
    {
        private UIManager manager;

        private void AdjustConfig()
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/UIConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("UIConfig.asset", fullPath);
            }

            UIConfig config = UIConfig.Get();
            string samplesPath = EditorFileUtils.GetSamplesPath().Replace(Path.GetFullPath("."), "").RemoveFirstCount();
            string windowRootPath = Path.Combine(samplesPath, "UISystem/Prefabs/Windows");
            AddressableUtils.CreateOrMoveEntry(windowRootPath, config.windowBundlePath);
#endif
        }

        private void Awake()
        {
            AdjustConfig();
        }

        private void Start()
        {
            manager = Global.GetService<UIManager>();
        }

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = Screen.height / 50;
            for (int i = 0; i < 10; i++)
            {
                string windowName = string.Concat("TestWindow", i);
                bool isShowWindow = manager.HasWindow(windowName);
                string buttonName = isShowWindow ? "CloseWindow" : "ShowWindow";
                if (GUILayout.Button($"{buttonName}[{windowName}]", style))
                {
                    if (manager.HasWindow(windowName))
                    {
                        manager.CloseWindow(windowName);
                        // manager.HideWindow(windowName);
                    }
                    else
                    {
                        Data<int> data = ReferencePool.Get<Data<int>>();
                        data.item = i;
                        manager.ShowWindow(windowName, data);
                    }
                }
            }

            if (GUILayout.Button($"CloseAllWindow", style))
            {
                manager.CloseAllWindow();
            }
        }
    }
}