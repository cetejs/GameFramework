using System.Collections.Generic;
using System.Text;

namespace GameFramework
{
    internal class PersistentJsonStorage : IPersistentStorage
    {
        private string savePath;
        private Dictionary<string, string> data;
        private List<string> tempKeys = new List<string>();
        private StorageAsyncOperation operation;

        public string Name { get; private set; }

        public PersistentState State { get; private set; }

        public bool IsValid
        {
            get { return State == PersistentState.Saving || State == PersistentState.Completed; }
        }

        void IPersistentStorage.Load(string storageName)
        {
            if (State == PersistentState.Loading || State == PersistentState.Saving)
            {
                return;
            }

            Name = storageName;
            State = PersistentState.Loading;
            savePath = PersistentSetting.Instance.GetSavePath(storageName);
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = FileUtils.ReadAllBytes(savePath);
                string json = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
                ReadToData(json);
            }
            else
            {
                string json = FileUtils.ReadAllText(savePath);
                ReadToData(json);
            }

            State = PersistentState.Completed;
        }

        StorageAsyncOperation IPersistentStorage.LoadAsync(string storageName)
        {
            if (operation == null)
            {
                operation = new StorageAsyncOperation(this);
            }

            if (State == PersistentState.Loading || State == PersistentState.Saving)
            {
                return operation;
            }

            Name = storageName;
            State = PersistentState.Loading;
            savePath = PersistentSetting.Instance.GetSavePath(storageName);
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                FileUtils.ReadAllBytesAsync(savePath, bytes =>
                {
                    string json = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
                    ReadToData(json);
                    State = PersistentState.Completed;
                    operation.Completed();
                });
            }
            else
            {
                FileUtils.ReadAllTextAsync(savePath, json =>
                {
                    ReadToData(json);
                    State = PersistentState.Completed;
                    operation.Completed();
                });
            }

            return operation;
        }

        void IPersistentStorage.Unload()
        {
            savePath = null;
            data = null;
            Name = null;
            tempKeys = null;
            operation = null;
            Name = null;
            State = PersistentState.None;
        }

        private void ReadToData(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                data = JsonUtils.ToObject<Dictionary<string, string>>(json);
            }

            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
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

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public StorageAsyncOperation SaveAsync()
        {
            if (operation == null)
            {
                operation = new StorageAsyncOperation(this);
            }

            if (State != PersistentState.Completed)
            {
                return operation;
            }

            State = PersistentState.Saving;
            string json = JsonUtils.ToJson(data);
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = CryptoUtils.Aes.EncryptStringToBytes(json, PersistentSetting.Instance.Password);
                FileUtils.WriteAllBytesAsync(savePath, bytes, () =>
                {
                    State = PersistentState.Completed;
                    operation.Completed();
                });
            }
            else
            {
                FileUtils.WriteAllTextAsync(savePath, json, () =>
                {
                    State = PersistentState.Completed;
                    operation.Completed();
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                });
            }

            return operation;
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (!IsValid)
            {
                return defaultValue;
            }

            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("Persistent get data is fail, because key is invalid");
                return defaultValue;
            }

            if (!HasKey(key))
            {
                return defaultValue;
            }

            if (data.TryGetValue(key, out string value))
            {
                return JsonUtils.ConvertToObject<T>(value);
            }

            return defaultValue;
        }

        public void SetData<T>(string key, T value)
        {
            if (!IsValid)
            {
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("Persistent set data is fail, because key is invalid");
                return;
            }

            if (value == null)
            {
                GameLogger.LogError("Persistent set data is fail, because value is invalid");
                return;
            }

            string json = JsonUtils.ConvertToJson(value);
            data[key] = json;
        }

        public string[] GetAllKeys()
        {
            if (!IsValid)
            {
                return null;
            }

            int i = 0;
            string[] result = new string[data.Count];
            foreach (string key in data.Keys)
            {
                result[i++] = key;
            }

            return result;
        }

        public void GetAllKeys(List<string> result)
        {
            result.Clear();
            if (!IsValid)
            {
                return;
            }

            if (result.Capacity < data.Count)
            {
                result.Capacity = data.Count;
            }

            result.AddRange(data.Keys);
        }

        public bool HasKey(string key)
        {
            if (!IsValid)
            {
                return false;
            }

            return data.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            if (!IsValid)
            {
                return;
            }

            data.Remove(key);
        }

        public void DeleteNode(string key)
        {
            if (!IsValid)
            {
                return;
            }

            GetAllKeys(tempKeys);
            for (int i = 0; i < data.Count; i++)
            {
                if (tempKeys[i].StartsWith(key))
                {
                    DeleteKey(tempKeys[i]);
                }
            }
        }

        public void DeleteAll()
        {
            if (State != PersistentState.Completed)
            {
                return;
            }

            data.Clear();
            FileUtils.DeleteFile(savePath);
#if UNITY_EDITOR
            FileUtils.DeleteFile(StringUtils.Concat(savePath, ".meta"));
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}