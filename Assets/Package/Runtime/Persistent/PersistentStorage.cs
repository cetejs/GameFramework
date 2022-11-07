using System;
using System.Collections.Generic;
using GameFramework.Generic;
using GameFramework.Utils;

namespace GameFramework.Persistent
{
    internal class PersistentStorage : IPersistentStorage
    {
        private readonly IPersistentHandler handler;
        private readonly List<string> keys;
        private readonly Dictionary<string, string> dict;

        public PersistentStorage(IPersistentHandler handler)
        {
            this.handler = handler;
            keys = new List<string>();
            dict = new Dictionary<string, string>();
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("key is invalid");
                return defaultValue;
            }

            if (!handler.HasKey(key))
            {
                return defaultValue;
            }

            string value = handler.GetString(key, null);
            return JsonUtils.ConvertToObject<T>(value);
        }

        public void SetData<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("key is invalid");
                return;
            }

            if (value == null)
            {
                GameLogger.LogError("value is invalid");
                return;
            }

            string json = JsonUtils.ConvertToJson(value);
            handler.SetString(key, json);
        }

        public string GetString(string key, string defaultValue)
        {
            return handler.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            handler.SetString(key, value);
        }

        public string[] GetAllKeys()
        {
            return handler.GetAllKeys();
        }

        public void GetAllKeys(List<string> results)
        {
            handler.GetAllKeys(results);
        }

        public bool HasKey(string key)
        {
            return handler.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            handler.DeleteKey(key);
        }

        public void DeleteNodeKey(string key)
        {
            handler.GetAllKeys(keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].StartsWith(key))
                {
                    handler.DeleteKey(keys[i]);
                }
            }
        }

        public void ImportData(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            try
            {
                Dictionary<string, string> tempDict = JsonUtils.ToObject<Dictionary<string, string>>(json);
                foreach (KeyValuePair<string, string> kvPair in tempDict)
                {
                    handler.SetString(kvPair.Key, kvPair.Value);
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogError($"DevCmdItem is thrown exception : {ex} {this}");
            }
        }

        public string ExportData(string key)
        {
            if (handler.HasKey(key))
            {
                dict.Clear();
                dict.Add(key, handler.GetString(key, null));
                return JsonUtils.ToJson(dict);
            }

            return null;
        }

        public string ExportNodeData(string key)
        {
            handler.GetAllKeys(keys);
            dict.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].StartsWith(key))
                {
                    dict.Add(keys[i], handler.GetString(keys[i], null));
                }
            }

            if (dict.Count > 0)
            {
                return JsonUtils.ToJson(dict);
            }

            return null;
        }

        public string ExportAllData()
        {
            handler.GetAllKeys(keys);
            dict.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], handler.GetString(keys[i], null));
            }

            if (dict.Count > 0)
            {
                return JsonUtils.ToJson(dict);
            }

            return null;
        }

        public void DeleteAll()
        {
            handler.DeleteAll();
        }

        public void Save()
        {
            handler.Save();
        }
    }
}