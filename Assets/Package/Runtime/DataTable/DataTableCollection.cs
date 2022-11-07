using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework.DataTableService
{
    public partial class DataTableManager
    {
        private class DataTableCollection
        {
            private readonly Dictionary<string, IDataTable> dataTableDict = new Dictionary<string, IDataTable>();
            private Dictionary<string, string> rawDataDict;
            private List<string> allKeys;

            public async void PreloadTable<T>() where T : IDataTable
            {
                if (rawDataDict != null)
                {
                    return;
                }

                string text = await LoadText<T>();
                await Task.Run(() =>
                {
                    ReadRawData(text);
                });
            }

            public async void ReloadTable<T>() where T : IDataTable
            {
                string text = await LoadText<T>();
                await Task.Run(() =>
                {
                    ReadRawData(text);
                });
            }

            public string[] GetAllKeys<T>() where T : class, IDataTable, new()
            {
                if (allKeys == null)
                {
                    TextAsset textAsset = LoadAsset<T>().WaitForCompletion();
                    ReadRawData(textAsset.text);
                    Addressables.Release(textAsset);
                }

                return allKeys.ToArray();
            }

            public async Task<string[]> GetAllKeysAsync<T>() where T : class, IDataTable, new()
            {
                if (allKeys == null)
                {
                    string text = await LoadText<T>();
                    await Task.Run(() =>
                    {
                        ReadRawData(text);
                    });
                }

                return allKeys.ToArray();
            }

            public T GetTable<T>(string id) where T : class, IDataTable, new()
            {
                if (dataTableDict.TryGetValue(id, out IDataTable table))
                {
                    return (T) table;
                }

                if (rawDataDict != null)
                {
                    return ReadTableFromRawData<T>(id);
                }

                TextAsset textAsset = LoadAsset<T>().WaitForCompletion();
                ReadRawData(textAsset.text);
                Addressables.Release(textAsset);
                return ReadTableFromRawData<T>(id);
            }

            public async Task<T> GetTableAsync<T>(string id) where T : class, IDataTable, new()
            {
                if (dataTableDict.TryGetValue(id, out IDataTable table))
                {
                    return (T) table;
                }

                if (rawDataDict != null)
                {
                    return ReadTableFromRawData<T>(id);
                }

                string text = await LoadText<T>();
                await Task.Run(() =>
                {
                    ReadRawData(text);
                });

                return ReadTableFromRawData<T>(id);
            }

            private AsyncOperationHandle<TextAsset> LoadAsset<T>() where T : IDataTable
            {
                string fullPath = string.Concat(DataTableConfig.Get().tableBundlePath, "/", typeof(T).Name, ".txt");
                return Addressables.LoadAssetAsync<TextAsset>(fullPath);
            }

            private async Task<string> LoadText<T>() where T : IDataTable
            {
                TextAsset textAsset = await LoadAsset<T>().Task;
                string text = textAsset.text;
                Addressables.Release(textAsset);
                return text;
            }

            private void ReadRawData(string text)
            {
                string[] lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (rawDataDict == null)
                {
                    allKeys = new List<string>(lines.Length);
                    rawDataDict = new Dictionary<string, string>(lines.Length);
                }
                else
                {
                    allKeys.Clear();
                    rawDataDict.Clear();
                }

                foreach (string line in lines)
                {
                    int index = line.IndexOf(",", StringComparison.Ordinal);
                    string rawId = line.Substring(0, index);
                    allKeys.Add(rawId);
                    rawDataDict.Add(rawId, line);
                }
            }

            private T ReadTableFromRawData<T>(string id) where T : class, IDataTable, new()
            {
                if (rawDataDict.TryGetValue(id, out string input))
                {
                    T table = new T();
                    table.Read(input);
                    rawDataDict.Remove(id);
                    dataTableDict.Add(id, table);
                    return table;
                }

                return null;
            }
        }
    }
}