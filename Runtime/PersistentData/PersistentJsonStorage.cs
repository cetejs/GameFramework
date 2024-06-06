using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework
{
    internal class PersistentJsonStorage : IPersistentStorage
    {
        private string savePath;
        private Dictionary<string, string> data = new Dictionary<string, string>();
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
                string text = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
                ReadToData(text);
            }
            else
            {
                string text = FileUtils.ReadAllText(savePath);
                ReadToData(text);
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
                    if (State == PersistentState.Loading)
                    {
                        string text = CryptoUtils.Aes.DecryptStringFromBytes(bytes, PersistentSetting.Instance.Password);
                        ReadToData(text);
                        State = PersistentState.Completed;
                        operation.Completed();
                    }
                });
            }
            else
            {
                FileUtils.ReadAllTextAsync(savePath, text =>
                {
                    if (State == PersistentState.Loading)
                    {
                        ReadToData(text);
                        State = PersistentState.Completed;
                        operation.Completed();
                    }
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

        private void ReadToData(string text)
        {
            data.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] map = line.Split(';');
                    data.Add(map[0], map[1]);
                }
            }
        }

        private string WriteToText()
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (KeyValuePair<string, string> kvPair in data)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append("\n");
                }

                builder.Append(kvPair.Key);
                builder.Append(";");
                builder.Append(kvPair.Value);
            }

            return builder.ToString();
        }

        public void Save()
        {
            if (State != PersistentState.Completed)
            {
                return;
            }

            State = PersistentState.Saving;
            string text = WriteToText();
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = CryptoUtils.Aes.EncryptStringToBytes(text, PersistentSetting.Instance.Password);
                FileUtils.WriteAllBytes(savePath, bytes);
            }
            else
            {
                FileUtils.WriteAllText(savePath, text);
            }

            State = PersistentState.Completed;
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
            string text = WriteToText();
            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                byte[] bytes = CryptoUtils.Aes.EncryptStringToBytes(text, PersistentSetting.Instance.Password);
                FileUtils.WriteAllBytesAsync(savePath, bytes, () =>
                {
                    if (State == PersistentState.Saving)
                    {
                        State = PersistentState.Completed;
                        operation.Completed();
                    }
                });
            }
            else
            {
                FileUtils.WriteAllTextAsync(savePath, text, () =>
                {
                    if (State == PersistentState.Saving)
                    {
                        State = PersistentState.Completed;
                        operation.Completed();
                    }
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
    }
}