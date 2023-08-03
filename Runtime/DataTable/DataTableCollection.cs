using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public partial class DataTableManager
    {
        private class DataTableCollection
        {
            private Type tableType;
            private string loadPath;
            private bool loaded;
            private Dictionary<string, IDataTable> dataTables = new Dictionary<string, IDataTable>();
            private Dictionary<string, string> rawDataRows = new Dictionary<string, string>();
            private List<string> allKeys = new List<string>();

            public DataTableCollection(Type tableType)
            {
                this.tableType = tableType;
                loadPath = PathUtils.Combine(DataTableSetting.Instance.LoadTablePath, tableType.Name);
            }

            public void PreloadTable()
            {
                if (loaded)
                {
                    return;
                }

                LoadTextAsync(ReadRawData);
            }

            public void ReloadTable()
            {
                UnloadTable();
                ReadRawData(LoadText());
            }

            public void ReloadTableAsync()
            {
                UnloadTable();
                LoadTextAsync(ReadRawData);
            }
            
            public void UnloadTable()
            {
                loaded = false;
                dataTables.Clear();
                allKeys.Clear();
                rawDataRows.Clear();
                AssetManager.Instance.UnloadAsset(loadPath);
            }

            public string[] GetAllKeys()
            {
                if (!loaded)
                {
                    ReadRawData(LoadText());
                }

                return allKeys?.ToArray();
            }

            public void GetAllKeysAsync(Action<string[]> callBack)
            {
                if (!loaded)
                {
                    LoadTextAsync(text =>
                    {
                        ReadRawData(text);
                        callBack?.Invoke(allKeys?.ToArray());
                    });
                }
                else
                {
                    callBack?.Invoke(allKeys.ToArray());
                }
            }

            public void GetAllKeys(List<string> results)
            {
                if (!loaded)
                {
                    ReadRawData(LoadText());
                }

                results.Clear();
                results.AddRange(allKeys);
            }

            public void GetAllKeysAsync(List<string> results, Action<List<string>> callBack)
            {
                results.Clear();
                if (!loaded)
                {
                    LoadTextAsync(text =>
                    {
                        ReadRawData(text);
                        results.AddRange(allKeys);
                    });
                }
                else
                {
                    callBack?.Invoke(results);
                }
            }

            public T GetTable<T>(string id) where T : class, IDataTable, new()
            {
                if (dataTables.TryGetValue(id, out IDataTable table))
                {
                    return table as T;
                }

                if (loaded)
                {
                    return ReadTableFromRawData<T>(id);
                }

                ReadRawData(LoadText());
                return ReadTableFromRawData<T>(id);
            }

            public void GetTableAsync<T>(string id, Action<T> callback) where T : class, IDataTable, new()
            {
                if (dataTables.TryGetValue(id, out IDataTable table))
                {
                    callback?.Invoke(table as T);
                    return;
                }

                if (loaded)
                {
                    callback?.Invoke(ReadTableFromRawData<T>(id));
                    return;
                }

                LoadTextAsync(text =>
                {
                    ReadRawData(text);
                    callback?.Invoke(ReadTableFromRawData<T>(id));
                });
            }

            private string LoadText()
            {
                TextAsset asset = AssetManager.Instance.LoadAsset<TextAsset>(loadPath);
                if (asset == null)
                {
                    GameLogger.LogError($"LoadText {loadPath} is fail");
                    return null;
                }

                DataTableSetting setting = DataTableSetting.Instance;
                if (setting.EncryptionType == EncryptionType.AES)
                {
                    return EncryptionUtils.AES.DecryptFromBytes(asset.bytes, setting.Password);
                }

                return asset.text;
            }

            private void LoadTextAsync(Action<string> callback)
            {
                AssetAsyncOperation operation = AssetManager.Instance.LoadAssetAsync(loadPath);
                operation.OnCompleted += _ =>
                {
                    TextAsset asset = operation.GetResult<TextAsset>();
                    if (asset == null)
                    {
                        GameLogger.LogError($"LoadText {loadPath} is fail");
                        return;
                    }

                    DataTableSetting setting = DataTableSetting.Instance;
                    if (setting.EncryptionType == EncryptionType.AES)
                    {
                        callback?.Invoke(EncryptionUtils.AES.DecryptFromBytes(asset.bytes, setting.Password));
                    }
                    else
                    {
                        callback?.Invoke(asset.text);
                    }
                };
            }

            private void ReadRawData(string text)
            {
                if (loaded)
                {
                    return;
                }

                loaded = true;
                allKeys.Clear();
                rawDataRows.Clear();
                string[] lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    int index = line.IndexOf(",", StringComparison.Ordinal);
                    string rawId = line.Substring(0, index);
                    allKeys.Add(rawId);
                    rawDataRows.Add(rawId, line);
                }
            }

            private T ReadTableFromRawData<T>(string id) where T : class, IDataTable, new()
            {
                if (rawDataRows.TryGetValue(id, out string input))
                {
                    T table = new T();
                    table.Read(input);
                    rawDataRows.Remove(id);
                    dataTables.Add(id, table);
                    return table;
                }

                return null;
            }
        }
    }
}