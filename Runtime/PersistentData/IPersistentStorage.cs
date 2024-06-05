using System.Collections.Generic;

namespace GameFramework
{
    public interface IPersistentStorage
    {
        string Name { get; }

        PersistentState State { get; }

        bool IsValid { get; }

        internal void Load(string storageName);

        internal StorageAsyncOperation LoadAsync(string storageName);

        internal void Unload();

        void Save();

        StorageAsyncOperation SaveAsync();

        T GetData<T>(string key, T defaultValue);

        void SetData<T>(string key, T value);

        string[] GetAllKeys();

        void GetAllKeys(List<string> result);

        bool HasKey(string key);

        void DeleteKey(string key);

        void DeleteNode(string key);

        void DeleteAll();
    }

    public enum PersistentState
    {
        None,
        Loading,
        Saving,
        Completed
    }
}