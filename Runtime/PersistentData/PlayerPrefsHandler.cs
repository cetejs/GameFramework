using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class PlayerPrefsHandler : IPersistentHandler
    {
        private int storageId;
        private int suffixIndex;
        private string globalKey;
        private List<string> localKeys;

        public PlayerPrefsHandler(int storageId)
        {
            this.storageId = storageId;
            suffixIndex = storageId.ToString().Length + 1;
            globalKey = StringUtils.Concat(nameof(PlayerPrefsHandler), storageId);
            string json = PlayerPrefs.GetString(globalKey, null);
            if (!string.IsNullOrEmpty(json))
            {
                localKeys = JsonUtils.ToObject<List<string>>(json);
            }
            else
            {
                localKeys = new List<string>();
            }
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(GetKey(key), defaultValue);
        }

        public void SetString(string key, string value)
        {
            key = GetKey(key);
            if (!localKeys.Contains(key))
            {
                localKeys.Add(key);
                PlayerPrefs.SetString(globalKey, JsonUtils.ToJson(localKeys));
            }

            PlayerPrefs.SetString(key, value);
        }

        public string[] GetAllKeys()
        {
            string[] results = new string[localKeys.Count];
            for (int i = 0; i < localKeys.Count; i++)
            {
                results[i] = localKeys[i].Remove(localKeys[i].Length - suffixIndex);
            }

            return results;
        }

        public void GetAllKeys(List<string> results)
        {
            results.Clear();
            if (results.Capacity < localKeys.Count)
            {
                results.Capacity = localKeys.Count;
            }

            for (int i = 0; i < localKeys.Count; i++)
            {
                results.Add(localKeys[i].Remove(localKeys[i].Length - suffixIndex));
            }
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(GetKey(key));
        }

        public void DeleteKey(string key)
        {
            key = GetKey(key);
            if (localKeys.Contains(key))
            {
                localKeys.Remove(key);
                PlayerPrefs.SetString(globalKey, JsonUtils.ToJson(localKeys));
            }

            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            localKeys.Clear();
            PlayerPrefs.DeleteAll();
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        private string GetKey(string key)
        {
            return StringUtils.Concat(key, "_", storageId);
        }
    }
}