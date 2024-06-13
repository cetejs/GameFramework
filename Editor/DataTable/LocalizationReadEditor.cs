using System.Collections.Generic;
using System.IO;

namespace GameFramework
{
    internal static class LocalizationReadEditor
    {
        private static string languageType;
        private static Dictionary<string, int> languageKeyMap = new Dictionary<string, int>();
        private static Dictionary<string, List<string>> languageMap = new Dictionary<string, List<string>>();

        public static void ChangeLanguage(string type)
        {
            if (languageType == type)
            {
                return;
            }

            if (string.IsNullOrEmpty(languageType))
            {
                string languageKeyPath = PathUtils.Combine("Assets", DataTableSetting.Instance.LoadLocalizationPath, "LanguageKey.bytes");
                if (!FileUtils.Exists(languageKeyPath))
                {
                    GameLogger.LogError($"Load localization {languageKeyPath} is fail");
                    return;
                }

                using (FileStream stream = new FileStream(languageKeyPath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        languageKeyMap.Clear();
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            languageKeyMap.Add(reader.ReadString(), i);
                        }
                    }
                }
            }

            languageType = type;
            if (!languageMap.TryGetValue(type, out List<string> languages))
            {
                string languagePath = PathUtils.Combine("Assets", DataTableSetting.Instance.LoadLocalizationPath, type, "Language.bytes");
                if (!FileUtils.Exists(languagePath))
                {
                    GameLogger.LogError($"Load localization {languagePath} is fail");
                    return;
                }

                using (FileStream stream = new FileStream(languagePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        languages = new List<string>();
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            languages.Add(reader.ReadString());
                        }

                        languageMap.Add(type, languages);
                    }
                }
            }
        }

        public static string GetLanguage(string key)
        {
            if (string.IsNullOrEmpty(languageType))
            {
                return string.Empty;
            }

            if (!languageMap.TryGetValue(languageType, out List<string> languages))
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

        public static void Reset()
        {
            languageType = string.Empty;
            languageKeyMap.Clear();
            languageMap.Clear();
        }
    }
}