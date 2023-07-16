using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class DataTableManager : PersistentSingleton<DataTableManager>
    {
        private Dictionary<Type, DataTableCollection> tableCollections = new Dictionary<Type, DataTableCollection>();

        public void PreloadTable<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().PreloadTable();
        }

        public void ReloadTable<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().ReloadTable();
        }

        public void ReloadTableAsync<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().ReloadTableAsync();
        }

        public bool HasTable<T>(string id) where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetTable<T>(id) != null;
        }

        public string[] GetAllKeys<T>() where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetAllKeys();
        }

        public void GetAllKeysAsync<T>(Action<string[]> callback) where T : class, IDataTable, new()
        {
            GetDataTableCollection<T>().GetAllKeysAsync(callback);
        }

        public void GetAllKeys<T>(List<string> result) where T : class, IDataTable, new()
        {
            GetDataTableCollection<T>().GetAllKeys(result);
        }

        public void GetAllKeysAsync<T>(List<string> result, Action<List<string>> callback) where T : class, IDataTable, new()
        {
            GetDataTableCollection<T>().GetAllKeysAsync(result, callback);
        }

        public T GetTable<T>(string id) where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetTable<T>(id);
        }

        public void GetTableAsync<T>(string id, Action<T> callback) where T : class, IDataTable, new()
        {
            GetDataTableCollection<T>().GetTableAsync<T>(id, callback);
        }

        public void UnloadAllTables()
        {
            foreach (DataTableCollection collection in tableCollections.Values)
            {
                collection.UnloadTable();
            }

            tableCollections.Clear();
        }

        private DataTableCollection GetDataTableCollection<T>() where T : IDataTable
        {
            Type key = typeof(T);
            if (!tableCollections.TryGetValue(key, out DataTableCollection collection))
            {
                collection = new DataTableCollection(typeof(T));
                tableCollections.Add(key, collection);
            }

            return collection;
        }
    }
}