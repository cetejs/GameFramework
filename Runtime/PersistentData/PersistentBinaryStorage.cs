using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameFramework
{
    public class PersistentBinaryStorage : IPersistentStorage
    {
        private string savePath;
        private Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
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
            byte[] bytes = FileUtils.ReadAllBytes(savePath);
            ReadToData(bytes);
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
            FileUtils.ReadAllBytesAsync(savePath, bytes =>
            {
                if (State == PersistentState.Loading)
                {
                    ReadToData(bytes);
                    State = PersistentState.Completed;
                    operation.Completed();
                }
            });

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

        private void ReadToData(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                bytes = CryptoUtils.Aes.DecryptBytesFromBytes(bytes, PersistentSetting.Instance.Password);
            }

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    try
                    {
                        int count = reader.ReadInt32();
                        if (count > 0)
                        {
                            data.Clear();
                            data.EnsureCapacity(count);
                            List<int> lengths = new List<int>();
                            for (int i = 0; i < count; i++)
                            {
                                lengths.Add(reader.ReadInt32());
                            }

                            for (int i = 0; i < lengths.Count; i++)
                            {
                                string key = reader.ReadString();
                                byte[] value = new byte[lengths[i]];
                                Array.Copy(bytes, stream.Position, value, 0, value.Length);
                                data.Add(key, value);
                                stream.Seek(lengths[i], SeekOrigin.Current);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        data = new Dictionary<string, byte[]>();
                        GameLogger.LogException(ex);
                    }
                }
            }
        }

        private byte[] WriteToBinary()
        {
            byte[] bytes = null;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    try
                    {
                        writer.Write(data.Count);

                        foreach (KeyValuePair<string, byte[]> kvPair in data)
                        {
                            writer.Write(kvPair.Value.Length);
                        }

                        foreach (KeyValuePair<string, byte[]> kvPair in data)
                        {
                            writer.Write(kvPair.Key);
                            writer.Write(kvPair.Value);
                        }

                        bytes = stream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        GameLogger.LogException(ex);
                    }
                }
            }

            if (PersistentSetting.Instance.CryptoType == CryptoType.AES)
            {
                bytes = CryptoUtils.Aes.DecryptBytesFromBytes(bytes, PersistentSetting.Instance.Password);
            }

            return bytes;
        }

        public void Save()
        {
            if (State != PersistentState.Completed)
            {
                return;
            }

            State = PersistentState.Saving;
            byte[] bytes = WriteToBinary();
            FileUtils.WriteAllBytes(savePath, bytes);
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
            byte[] bytes = WriteToBinary();
            FileUtils.WriteAllBytesAsync(savePath, bytes, () =>
            {
                if (State == PersistentState.Saving)
                {
                    State = PersistentState.Completed;
                    operation.Completed();
                }
            });

            return operation;
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                GameLogger.LogError("Persistent get data is fail, because key is invalid");
                return defaultValue;
            }

            if (!HasKey(key))
            {
                return defaultValue;
            }

            if (data.TryGetValue(key, out byte[] bytes))
            {
                defaultValue = BinaryUtils.ConvertToObject<T>(bytes);
            }

            return defaultValue;
        }

        public void SetData<T>(string key, T value)
        {
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

            data[key] = BinaryUtils.ConvertToBinary(value);
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