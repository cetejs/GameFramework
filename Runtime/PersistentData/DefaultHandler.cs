using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework
{
    internal class DefaultHandler : IPersistentHandler
    {
        private string savePath;
        private Dictionary<string, string> data;

        public DefaultHandler(int storageId = 0)
        {
            savePath = PersistentSetting.Instance.GetSavePath(storageId);

            string json;
            if (PersistentSetting.Instance.EncryptionType == EncryptionType.AES)
            {
                byte[] bytes = FileUtils.ReadAllBytes(savePath);
                json = EncryptionUtils.AES.DecryptFromBytes(bytes, PersistentSetting.Instance.Password);
            }
            else
            {
                json = FileUtils.ReadAllText(savePath);
            }

            if (!string.IsNullOrEmpty(json))
            {
                data = JsonUtils.ToObject<Dictionary<string, string>>(json);
            }
            else
            {
                data = new Dictionary<string, string>();
            }
        }

        public string GetString(string key, string defaultValue)
        {
            if (data.TryGetValue(key, out string value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            data[key] = value;
        }

        public string[] GetAllKeys()
        {
            int i = 0;
            string[] results = new string[data.Count];
            foreach (string key in data.Keys)
            {
                results[i++] = key;
            }

            return results;
        }

        public void GetAllKeys(List<string> results)
        {
            results.Clear();
            if (results.Capacity < data.Count)
            {
                results.Capacity = data.Count;
            }

            results.AddRange(data.Keys);
        }

        public bool HasKey(string key)
        {
            return data.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            data.Remove(key);
        }

        public void DeleteAll()
        {
            data.Clear();
            FileUtils.DeleteFile(savePath);
#if UNITY_EDITOR
            FileUtils.DeleteFile(StringUtils.Concat(savePath, ".meta"));
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public void Save()
        {
            string json = JsonUtils.ToJson(data);
            byte[] bytes;
            if (PersistentSetting.Instance.EncryptionType == EncryptionType.AES)
            {
                bytes = EncryptionUtils.AES.EncryptToBytes(json, PersistentSetting.Instance.Password);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(json);
            }

            FileUtils.WriteAllBytes(savePath, bytes);
        }

        public void SaveAsync(Action callback)
        {
            string json = JsonUtils.ToJson(data);
            if (PersistentSetting.Instance.EncryptionType == EncryptionType.AES)
            {
                byte[] bytes = EncryptionUtils.AES.EncryptToBytes(json, PersistentSetting.Instance.Password);
                FileUtils.WriteAllBytesAsync(savePath, bytes, callback);
            }
            else
            {
                FileUtils.WriteAllTextAsync(savePath, json, callback);
            }
        }
    }
}