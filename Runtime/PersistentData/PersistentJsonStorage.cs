using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework
{
    internal class PersistentJsonStorage : IPersistentStorage
    {
        private string savePath;
        private Dictionary<string, string> data;
        private List<string> tempKeys = new List<string>();
        private Dictionary<string, string> tempData = new Dictionary<string, string>();

        public string Name { get; private set; }

        public PersistentState State { get; private set; }

        public void Load(string storageName)
        {
            if (State == PersistentState.Loading || State == PersistentState.Saving)
            {
                return;
            }

            Name = storageName;
            State = PersistentState.Loading;
            savePath = PersistentSetting.Instance.GetSavePath(storageName);
            string json;
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = FileUtils.ReadAllBytes(savePath);
                json = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
            }
            else
            {
                json = FileUtils.ReadAllText(savePath);
            }

            LoadCompleted(json);
        }

        public void LoadAsync(string storageName, Action<PersistentState> callback)
        {
            if (State == PersistentState.Loading || State == PersistentState.Saving)
            {
                callback?.Invoke(State);
                return;
            }

            Name = storageName;
            State = PersistentState.Loading;
            savePath = PersistentSetting.Instance.GetSavePath(storageName);
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                FileUtils.ReadAllBytesAsync(savePath, bytes =>
                {
                    string json = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
                    LoadCompleted(json);
                    callback?.Invoke(State);
                });
            }
            else
            {
                FileUtils.ReadAllTextAsync(savePath, json =>
                {
                    LoadCompleted(json);
                    callback?.Invoke(State);
                });
            }
        }

        public void Unload()
        {
            savePath = null;
            data = null;
            Name = null;
            State = PersistentState.None;
        }

        private void LoadCompleted(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                data = JsonUtils.ToObject<Dictionary<string, string>>(json);
            }
            else
            {
                data = new Dictionary<string, string>();
            }

            State = PersistentState.Completed;
        }

        public void Save()
        {
            if (State != PersistentState.Completed)
            {
                return;
            }

            State = PersistentState.Saving;
            string json = JsonUtils.ToJson(data);
            byte[] bytes;
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                bytes = CryptoUtils.Aes.EncryptStringToBytes(json, PersistentSetting.Instance.Password);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(json);
            }

            FileUtils.WriteAllBytes(savePath, bytes);
            State = PersistentState.Completed;
        }

        public void SaveAsync(Action<PersistentState> callback)
        {
            if (State != PersistentState.Completed)
            {
                callback?.Invoke(State);
                return;
            }

            State = PersistentState.Saving;
            string json = JsonUtils.ToJson(data);
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = CryptoUtils.Aes.EncryptStringToBytes(json, PersistentSetting.Instance.Password);
                FileUtils.WriteAllBytesAsync(savePath, bytes, () =>
                {
                    State = PersistentState.Completed;
                    callback?.Invoke(State);
                });
            }
            else
            {
                FileUtils.WriteAllTextAsync(savePath, json, () =>
                {
                    State = PersistentState.Completed;
                    callback?.Invoke(State);
                });
            }
        }

        private string GetString(string key, string defaultValue)
        {
            if (data.TryGetValue(key, out string value))
            {
                return value;
            }

            return defaultValue;
        }

        private void SetString(string key, string value)
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

        public bool DeleteKey(string key)
        {
            return data.Remove(key);
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

        public T GetData<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("Key is invalid");
                return defaultValue;
            }

            if (!HasKey(key))
            {
                return defaultValue;
            }

            string value = GetString(key, null);
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
            SetString(key, json);
        }

        public void DeleteNodeKey(string key)
        {
            GetAllKeys(tempKeys);
            for (int i = 0; i < data.Count; i++)
            {
                if (tempKeys[i].StartsWith(key))
                {
                    DeleteKey(tempKeys[i]);
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
                    SetString(kvPair.Key, kvPair.Value);
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
            }
        }

        public string ExportData(string key)
        {
            if (HasKey(key))
            {
                tempData.Clear();
                tempData.Add(key, GetString(key, null));
                return JsonUtils.ToJson(tempData);
            }

            return null;
        }

        public string ExportNodeData(string key)
        {
            GetAllKeys(tempKeys);
            tempData.Clear();
            for (int i = 0; i < tempKeys.Count; i++)
            {
                if (tempKeys[i].StartsWith(key))
                {
                    tempData.Add(tempKeys[i], GetString(tempKeys[i], null));
                }
            }

            if (tempData.Count > 0)
            {
                return JsonUtils.ToJson(tempData);
            }

            return null;
        }

        public string ExportAllData()
        {
            if (data.Count > 0)
            {
                return JsonUtils.ToJson(data);
            }

            return null;
        }
    }
}