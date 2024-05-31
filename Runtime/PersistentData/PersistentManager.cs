using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class PersistentManager : PersistentSingleton<PersistentManager>
    {
        private Dictionary<string, IPersistentStorage> storages = new Dictionary<string, IPersistentStorage>();
        public event Action<string> OnStorageLoading;
        public event Action<string> OnStorageSaving;

        public IPersistentStorage GetStorage(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is get fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage = Load(storageName);
            }

            return storage;
        }

        public IPersistentStorage Load()
        {
            return Load(PersistentSetting.Instance.DefaultStorageName);
        }

        public IPersistentStorage Load(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is load fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage = new PersistentJsonStorage();
                storages.Add(storageName, storage);
            }

            storage.Load(storageName);
            OnStorageLoading?.Invoke(storageName);
            return storage;
        }

        public IPersistentStorage LoadAsync(Action<PersistentState> callback)
        {
            return LoadAsync(PersistentSetting.Instance.DefaultStorageName, callback);
        }

        public IPersistentStorage LoadAsync(string storageName, Action<PersistentState> callback)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is load fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage = new PersistentJsonStorage();
                storages.Add(storageName, storage);
            }

            storage.LoadAsync(storageName, callback);
            OnStorageLoading?.Invoke(storageName);
            return storage;
        }

        public void Unload()
        {
            Unload(PersistentSetting.Instance.DefaultStorageName);
        }

        public void Unload(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is unload fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage.Unload();
                storages.Remove(storageName);
            }
            else
            {
                GameLogger.LogError($"Storage is unload fail, because storage {storageName} is not loaded");
            }
        }

        public void Save()
        {
            Save(PersistentSetting.Instance.DefaultStorageName);
        }

        public void Save(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is save fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                OnStorageSaving?.Invoke(storageName);
                storage.Save();
            }
            else
            {
                GameLogger.LogError($"Storage is save fail, because storage {storageName} is not loaded");
            }
        }

        public void SaveAsync(Action<PersistentState> callback)
        {
            SaveAsync(PersistentSetting.Instance.DefaultStorageName, callback);
        }

        public void SaveAsync(string storageName, Action<PersistentState> callback)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                GameLogger.LogError("Storage is save fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                OnStorageSaving?.Invoke(storageName);
                storage.SaveAsync(callback);
            }
            else
            {
                GameLogger.LogError($"Storage is save fail, because storage {storageName} is not loaded");
            }
        }

        public T GetData<T>(string key, T defaultValue = default)
        {
            return GetData<T>(PersistentSetting.Instance.DefaultStorageName, key, defaultValue);
        }

        public T GetData<T>(string storageName, string key, T defaultValue = default)
        {
            return GetStorage(storageName).GetData(key, defaultValue);
        }

        public void SetData<T>(string key, T value)
        {
            SetData(PersistentSetting.Instance.DefaultStorageName, key, value);
        }

        public void SetData<T>(string storageName, string key, T value)
        {
            GetStorage(storageName).SetData(key, value);
        }

        public string[] GetAllKeys()
        {
            return GetAllKeys(PersistentSetting.Instance.DefaultStorageName);
        }

        public string[] GetAllKeys(string storageName)
        {
            return GetStorage(storageName).GetAllKeys();
        }

        public void GetAllKeys(List<string> results)
        {
            GetAllKeys(PersistentSetting.Instance.DefaultStorageName, results);
        }

        public void GetAllKeys(string storageName, List<string> results)
        {
            GetStorage(storageName).GetAllKeys(results);
        }

        public bool HasKey(string key)
        {
            return HasKey(PersistentSetting.Instance.DefaultStorageName, key);
        }

        public bool HasKey(string storageName, string key)
        {
            return GetStorage(storageName).HasKey(key);
        }

        public void DeleteKey(string key)
        {
            DeleteKey(PersistentSetting.Instance.DefaultStorageName, key);
        }

        public void DeleteKey(string storageName, string key)
        {
            GetStorage(storageName).DeleteKey(key);
        }

        public void DeleteNodeKey(string key)
        {
            DeleteNodeKey(PersistentSetting.Instance.DefaultStorageName, key);
        }

        public void DeleteNodeKey(string storageName, string key)
        {
            GetStorage(storageName).DeleteNodeKey(key);
        }

        public void ImportData(string json)
        {
            ImportData(PersistentSetting.Instance.DefaultStorageName, json);
        }

        public void ImportData(string storageName, string json)
        {
            GetStorage(storageName).ImportData(json);
        }

        public string ExportData(string key)
        {
            return ExportData(PersistentSetting.Instance.DefaultStorageName, key);
        }

        public string ExportData(string storageName, string key)
        {
            return GetStorage(storageName).ExportData(key);
        }

        public string ExportNodeData(string key)
        {
            return ExportNodeData(PersistentSetting.Instance.DefaultStorageName, key);
        }

        public string ExportNodeData(string storageName, string key)
        {
            return GetStorage(storageName).ExportNodeData(key);
        }

        public string ExportAllData()
        {
            return ExportAllData(PersistentSetting.Instance.DefaultStorageName);
        }

        public string ExportAllData(string storageName)
        {
            return GetStorage(storageName).ExportAllData();
        }

        public void DeleteAll()
        {
            DeleteAll(PersistentSetting.Instance.DefaultStorageName);
        }

        public void DeleteAll(string storageName)
        {
            GetStorage(storageName).DeleteAll();
        }
    }
}