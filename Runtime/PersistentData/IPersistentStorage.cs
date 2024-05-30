using System;
using System.Collections.Generic;

namespace GameFramework
{
    public interface IPersistentStorage
    {
        string Name { get; }

        PersistentState State { get; }

        void Load(string storageName);

        void LoadAsync(string storageName, Action<PersistentState> callback);

        void Unload();

        void Save();

        void SaveAsync(Action<PersistentState> callback);

        T GetData<T>(string key, T defaultValue);

        void SetData<T>(string key, T value);

        string[] GetAllKeys();

        void GetAllKeys(List<string> results);

        bool HasKey(string key);

        bool DeleteKey(string key);

        void DeleteNodeKey(string key);

        void DeleteAll();

        void ImportData(string json);

        string ExportData(string key);

        string ExportNodeData(string key);

        string ExportAllData();
    }

    public enum PersistentState
    {
        None,
        Loading,
        Saving,
        Completed
    }
}