using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFramework.UIService
{
    public class UIConfig : ScriptableObject
    {
        public AssetReference windowRoot;
        public string windowBundlePath = "Prefabs/Windows";
        public string[] windowLayers = new string[]
        {
            "FullScreen",
            "Fix",
            "PopUp",
            "Top"
        };

        private static UIConfig instance;

        public static UIConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            UIConfig config = Resources.Load<UIConfig>("UIConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }
}