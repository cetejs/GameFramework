using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class PlayerPrefsHandler : IPersistentHandler
    {
        private int storageId;
        private string globalKey;
        private List<string> keys;
        private Dictionary<string, string> data;

        public PlayerPrefsHandler(int storageId)
        {
            this.storageId = storageId;
            int suffixIndex = storageId.ToString().Length + 1;
            globalKey = TryEncrypt(StringUtils.Concat(nameof(PlayerPrefsHandler), storageId));
            string json = PlayerPrefs.GetString(globalKey, null);
            if (!string.IsNullOrEmpty(json))
            {
                keys = JsonUtils.ToObject<List<string>>(TryDecrypt(json));
                foreach (string key in keys)
                {
                    data.Add(key.Remove(key.Length - suffixIndex), TryDecrypt(PlayerPrefs.GetString(key)));
                }
            }
            else
            {
                keys = new List<string>();
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
            PlayerPrefs.DeleteAll();
        }

        public void Save()
        {
            foreach (KeyValuePair<string, string> kvPair in data)
            {
                string key = StringUtils.Concat(kvPair.Key, "_", storageId);
                PlayerPrefs.SetString(TryEncrypt(key), TryEncrypt(kvPair.Value));
            }

            foreach (string key in keys)
            {
                if (!data.ContainsKey(key))
                {
                    PlayerPrefs.DeleteKey(StringUtils.Concat(key, "_", storageId));
                }
            }

            keys.Clear();
            keys.AddRange(data.Keys);
            PlayerPrefs.SetString(globalKey, TryEncrypt(JsonUtils.ToJson(keys)));

            PlayerPrefs.Save();
        }

        public void SaveAsync(Action callback)
        {
            Save();
            callback?.Invoke();
        }

        private string TryEncrypt(string text)
        {
            if (PersistentSetting.Instance.EncryptionType == EncryptionType.AES)
            {
                text = EncryptionUtils.AES.EncryptToString(text, PersistentSetting.Instance.Password);
            }

            return text;
        }

        private string TryDecrypt(string text)
        {
            if (PersistentSetting.Instance.EncryptionType == EncryptionType.AES)
            {
                text = EncryptionUtils.AES.DecryptFromString(text, PersistentSetting.Instance.Password);
            }

            return text;
        }
    }
}