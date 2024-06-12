using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        private Dictionary<string, int> languageKeyMap = new Dictionary<string, int>();
        private List<string> languages = new List<string>();
        private string languageType;

        public event Action<string> OnLanguageChanged;

        public string LanguageType
        {
            get { return languageType; }
        }

        public string GetLanguage(string key)
        {
            if (string.IsNullOrEmpty(languageType))
            {
                return string.Empty;
            }

            if (languageKeyMap.TryGetValue(key, out int index))
            {
                if (index >= 0 && index <= languages.Count)
                {
                    return languages[index];
                }
            }

            GameLogger.LogError($"Language not found {key}");
            return string.Empty;
        }

        public void ChangeLanguage(string type)
        {
            if (languageType == type)
            {
                return;
            }

            if (string.IsNullOrEmpty(languageType))
            {
                ReadLanguageKey(LoadText("LanguageKey"));
            }

            languageType = type;
            ReadLanguage(LoadText(StringUtils.Concat(type, "/Language")));
            OnLanguageChanged?.Invoke(type);
        }

        public void ChangeLanguageAsync(string type, Action callback)
        {
            if (languageType == type)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(languageType))
            {
                ReadLanguageKey(LoadText("LanguageKey"));
            }

            languageType = type;
            LoadTextAsync(StringUtils.Concat(languageType, "/Language"), bytes =>
            {
                ReadLanguage(bytes);
                OnLanguageChanged?.Invoke(type);
                callback?.Invoke();
            });
        }

        public void UnloadLanguage()
        {
            languageType = null;
            languageKeyMap.Clear();
            languages.Clear();
        }

        private byte[] LoadText(string path)
        {
            string loadPath = PathUtils.Combine(DataTableSetting.Instance.LoadLocalizationPath, path);
            TextAsset asset = AssetManager.Instance.LoadAsset<TextAsset>(loadPath);
            if (asset == null)
            {
                GameLogger.LogError($"Load localization {loadPath} is fail");
                return null;
            }

            DataTableSetting setting = DataTableSetting.Instance;
            if (setting.CryptoType == CryptoType.AES)
            {
                return CryptoUtils.Aes.DecryptBytesFromBytes(asset.bytes, setting.Password);
            }

            return asset.bytes;
        }

        private void LoadTextAsync(string path, Action<byte[]> callback)
        {
            string loadPath = PathUtils.Combine(DataTableSetting.Instance.LoadLocalizationPath, path);
            AssetAsyncOperation operation = AssetManager.Instance.LoadAssetAsync(loadPath);
            operation.OnCompleted += _ =>
            {
                TextAsset asset = operation.GetResult<TextAsset>();
                if (asset == null)
                {
                    GameLogger.LogError($"Load localization {loadPath} is fail");
                    return;
                }

                DataTableSetting setting = DataTableSetting.Instance;
                if (setting.CryptoType == CryptoType.AES)
                {
                    callback?.Invoke(CryptoUtils.Aes.DecryptBytesFromBytes(asset.bytes, setting.Password));
                }
                else
                {
                    callback?.Invoke(asset.bytes);
                }
            };
        }

        private void ReadLanguageKey(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    languageKeyMap.Clear();
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        languageKeyMap.Add(key, i);
                    }
                }
            }

            AssetManager.Instance.UnloadBundle(DataTableSetting.Instance.LoadLocalizationPath, true);
        }

        private void ReadLanguage(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    languages.Clear();
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string language = reader.ReadString();
                        languages.Add(language);
                    }
                }
            }

            AssetManager.Instance.UnloadBundle(PathUtils.Combine(DataTableSetting.Instance.LoadLocalizationPath, languageType), true);
        }
    }
}