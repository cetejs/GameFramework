using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class DataTableManager : PersistentSingleton<DataTableManager>
    {
        private Dictionary<Type, DataTableCollection> tableCollections = new Dictionary<Type, DataTableCollection>();

        public void PreloadTable(Type tableType, Action callback = null)
        {
            GetDataTableCollection(tableType).PreloadTable(callback);
        }

        public void ReloadTableAsync(Type tableType, Action callback = null)
        {
            GetDataTableCollection(tableType).ReloadTableAsync(callback);
        }

        public void PreloadTable<T>(Action callback = null) where T : IDataTable
        {
            GetDataTableCollection<T>().PreloadTable(callback);
        }

        public void ReloadTable<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().ReloadTable();
        }

        public void ReloadTableAsync<T>(Action callback = null) where T : IDataTable
        {
            GetDataTableCollection<T>().ReloadTableAsync(callback);
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
            return GetDataTableCollection(key);
        }

        private DataTableCollection GetDataTableCollection(Type key)
        {
            if (!tableCollections.TryGetValue(key, out DataTableCollection collection))
            {
                collection = new DataTableCollection(key);
                tableCollections.Add(key, collection);
            }

            return collection;
        }

        public override string ToString()
        {
            string result = "";
            bool first = true;
            foreach (DataTableCollection collection in tableCollections.Values)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result += "\n";
                }

                result += collection;
            }

            return result;
        }
    }
}