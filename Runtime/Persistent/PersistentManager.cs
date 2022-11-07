using System.Collections.Generic;
using GameFramework.Generic;
using UnityEngine;

namespace GameFramework.Persistent
{
    public class PersistentManager : PersistentService
    {
        private IPersistentStorage storage;

        private float autoSaveEndTime;
        private float AutoSaveTime { get; set; } = 1f;

        private IPersistentStorage Storage
        {
            get { return storage ??= new PersistentStorage(new PlayerPrefsHandler()); }
        }

        private void Update()
        {
            if (AutoSaveTime > 0.0f && Time.realtimeSinceStartup - autoSaveEndTime > AutoSaveTime)
            {
                autoSaveEndTime = Time.realtimeSinceStartup;
                Save();
            }
        }

        public T GetData<T>(string key, T defaultValue = default)
        {
            return Storage.GetData(key, defaultValue);
        }

        public void SetData<T>(string key, T value)
        {
            Storage.SetData(key, value);
        }

        public string GetString(string key, string defaultValue = default)
        {
            return Storage.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            Storage.SetString(key, value);
        }

        public string[] GetAllKeys()
        {
            return Storage.GetAllKeys();
        }

        public void GetAllKeys(List<string> results)
        {
            Storage.GetAllKeys(results);
        }

        public bool HasKey(string key)
        {
            return Storage.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            Storage.DeleteKey(key);
        }

        public void DeleteNodeKey(string key)
        {
            Storage.DeleteNodeKey(key);
        }

        public void ImportData(string json)
        {
            Storage.ImportData(json);
        }

        public string ExportData(string key)
        {
            return Storage.ExportData(key);
        }

        public string ExportNodeData(string key)
        {
            return Storage.ExportNodeData(key);
        }

        public string ExportAllData()
        {
            return Storage.ExportAllData();
        }

        public void DeleteAll()
        {
            Storage.DeleteAll();
        }

        public void Save()
        {
            Storage.Save();
        }
    }
}