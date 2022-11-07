using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Generic;

namespace GameFramework.DataTableService
{
    public partial class DataTableManager : Service
    {
        private readonly Dictionary<Type, DataTableCollection> dataTableCollections = new Dictionary<Type, DataTableCollection>();

        public void PreloadTable<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().PreloadTable<T>();
        }

        public void ReloadTable<T>() where T : IDataTable
        {
            GetDataTableCollection<T>().ReloadTable<T>();
        }

        public bool HasTable<T>(string id) where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetTable<T>(id) != null;
        }

        public async Task<bool> HasTableAsync<T>(string id) where T : class, IDataTable, new()
        {
            return await GetDataTableCollection<T>().GetTableAsync<T>(id) != null;
        }

        public string[] GetAllKeys<T>() where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetAllKeys<T>();
        }

        public async Task<string[]> GetAllKeysAsync<T>() where T : class, IDataTable, new()
        {
            return await GetDataTableCollection<T>().GetAllKeysAsync<T>();
        }

        public T GetTable<T>(string id) where T : class, IDataTable, new()
        {
            return GetDataTableCollection<T>().GetTable<T>(id);
        }

        public async Task<T> GetTableAsync<T>(string id) where T : class, IDataTable, new()
        {
            return await GetDataTableCollection<T>().GetTableAsync<T>(id);
        }

        public void Clear()
        {
            dataTableCollections.Clear();
        }

        private DataTableCollection GetDataTableCollection<T>() where T : IDataTable
        {
            Type key = typeof(T);
            if (!dataTableCollections.TryGetValue(key, out DataTableCollection collection))
            {
                collection = new DataTableCollection();
                dataTableCollections.Add(key, collection);
            }

            return collection;
        }
    }
}