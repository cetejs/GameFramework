using System;
using System.Collections.Generic;

namespace GameFramework
{
    internal class PersistentStorage : IPersistentStorage
    {
        private IPersistentHandler handler;
        private List<string> allKeys = new List<string>();
        private Dictionary<string, string> allData = new Dictionary<string, string>();

        public void SetHandler(IPersistentHandler handler)
        {
            this.handler = handler;
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("Key is invalid");
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
                GameLogger.LogError("Key is invalid");
                return;
            }

            if (value == null)
            {
                GameLogger.LogError("Value is invalid");
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
            handler.GetAllKeys(allKeys);
            for (int i = 0; i < allKeys.Count; i++)
            {
                if (allKeys[i].StartsWith(key))
                {
                    handler.DeleteKey(allKeys[i]);
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
                GameLogger.LogException(ex);
            }
        }

        public string ExportData(string key)
        {
            if (handler.HasKey(key))
            {
                allData.Clear();
                allData.Add(key, handler.GetString(key, null));
                return JsonUtils.ToJson(allData);
            }

            return null;
        }

        public string ExportNodeData(string key)
        {
            handler.GetAllKeys(allKeys);
            allData.Clear();
            for (int i = 0; i < allKeys.Count; i++)
            {
                if (allKeys[i].StartsWith(key))
                {
                    allData.Add(allKeys[i], handler.GetString(allKeys[i], null));
                }
            }

            if (allData.Count > 0)
            {
                return JsonUtils.ToJson(allData);
            }

            return null;
        }

        public string ExportAllData()
        {
            handler.GetAllKeys(allKeys);
            allData.Clear();
            for (int i = 0; i < allKeys.Count; i++)
            {
                allData.Add(allKeys[i], handler.GetString(allKeys[i], null));
            }

            if (allData.Count > 0)
            {
                return JsonUtils.ToJson(allData);
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