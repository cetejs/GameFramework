using System.Collections.Generic;
using GameFramework.Utils;
using UnityEngine;

namespace GameFramework.Persistent
{
    internal class PlayerPrefsHandler : IPersistentHandler
    {
        private static readonly string Key = typeof(PlayerPrefsHandler).FullName;
        private readonly List<string> keys;

        public PlayerPrefsHandler()
        {
            string json = PlayerPrefs.GetString(Key, null);
            if (!string.IsNullOrEmpty(json))
            {
                keys = JsonUtils.ToObject<List<string>>(json);
            }
            else
            {
                keys = new List<string>();
            }
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            if (!keys.Contains(key))
            {
                AddKeys(key);
            }

            PlayerPrefs.SetString(key, value);
        }

        public string[] GetAllKeys()
        {
            return keys.ToArray();
        }

        public void GetAllKeys(List<string> results)
        {
            results.Clear();
            results.AddRange(keys);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            if (keys.Contains(key))
            {
                RemoveKeys(key);
            }

            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            keys.Clear();
            PlayerPrefs.DeleteAll();
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        private void AddKeys(string key)
        {
            keys.Add(key);
            PlayerPrefs.SetString(Key, JsonUtils.ToJson(keys));
        }

        private void RemoveKeys(string key)
        {
            keys.Remove(key);
            PlayerPrefs.SetString(Key, JsonUtils.ToJson(keys));
        }
    }
}