using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class PersistentManager : PersistentSingleton<PersistentManager>
    {
        private float autoSaveEndTime;
        private IPersistentStorage storage = new PersistentStorage();
        public float AutoSaveTime { get; set; } = 1f;

        public StorageType StorageType { get; private set; }
        public int StorageId { get; private set; }

        private void OnEnable()
        {
            ChangeStorage(StorageType.Default);
        }

        private void Update()
        {
            if (AutoSaveTime > 0f && Time.realtimeSinceStartup - autoSaveEndTime > AutoSaveTime)
            {
                autoSaveEndTime = Time.realtimeSinceStartup;
                Save();
            }
        }

        public void ChangeStorage(StorageType type, int storageId = 0)
        {
            StorageId = storageId;
            StorageType = type;
            switch (type)
            {
                case StorageType.PlayerPrefs:
                    storage.SetHandler(new PlayerPrefsHandler(storageId));
                    break;
                default:
                    storage.SetHandler(new DefaultHandler(storageId));
                    break;
            }
        }

        public T GetData<T>(string key, T defaultValue = default)
        {
            return storage.GetData(key, defaultValue);
        }

        public void SetData<T>(string key, T value)
        {
            storage.SetData(key, value);
        }

        public string GetString(string key, string defaultValue = default)
        {
            return storage.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            storage.SetString(key, value);
        }

        public string[] GetAllKeys()
        {
            return storage.GetAllKeys();
        }

        public void GetAllKeys(List<string> results)
        {
            storage.GetAllKeys(results);
        }

        public bool HasKey(string key)
        {
            return storage.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            storage.DeleteKey(key);
        }

        public void DeleteNodeKey(string key)
        {
            storage.DeleteNodeKey(key);
        }

        public void ImportData(string json)
        {
            storage.ImportData(json);
        }

        public string ExportData(string key)
        {
            return storage.ExportData(key);
        }

        public string ExportNodeData(string key)
        {
            return storage.ExportNodeData(key);
        }

        public string ExportAllData()
        {
            return storage.ExportAllData();
        }

        public void DeleteAll()
        {
            storage.DeleteAll();
        }

        public void Save()
        {
            storage.Save();
        }

        public void SaveAsync(Action callback)
        {
            storage.SaveAsync(callback);
        }
    }

    public enum StorageType
    {
        Default,
        PlayerPrefs,
    }
}