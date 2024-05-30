using System;
using System.Collections.Generic;
using System.IO;
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
            private MemoryStream stream;
            private BinaryReader reader;
            private Dictionary<string, IDataTable> dataTables = new Dictionary<string, IDataTable>();
            private List<long> rowOffsets = new List<long>();
            private List<string> allKeys = new List<string>();

            public DataTableCollection(Type tableType)
            {
                this.tableType = tableType;
                loadPath = PathUtils.Combine(DataTableSetting.Instance.LoadTablePath, tableType.Name);
                stream = new MemoryStream();
                reader = new BinaryReader(stream);
            }

            ~DataTableCollection()
            {
                stream.Dispose();
                reader.Dispose();
            }

            public void PreloadTable(Action callback)
            {
                if (loaded)
                {
                    return;
                }

                LoadTextAsync(bytes =>
                {
                    ReadRawData(bytes);
                    callback?.Invoke();
                });
            }

            public void ReloadTable()
            {
                UnloadTable();
                ReadRawData(LoadText());
            }

            public void ReloadTableAsync(Action callback)
            {
                UnloadTable();
                LoadTextAsync(text =>
                {
                    ReadRawData(text);
                    callback?.Invoke();
                });
            }

            public void UnloadTable()
            {
                loaded = false;
                dataTables.Clear();
                allKeys.Clear();
                rowOffsets.Clear();
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

            private byte[] LoadText()
            {
                TextAsset asset = AssetManager.Instance.LoadAsset<TextAsset>(loadPath);
                if (asset == null)
                {
                    GameLogger.LogError($"LoadText {loadPath} is fail");
                    return null;
                }

                DataTableSetting setting = DataTableSetting.Instance;
                if (setting.CryptoType == CryptoType.AES)
                {
                    return CryptoUtils.Aes.DecryptBytesFromBytes(asset.bytes, setting.Password);
                }

                return asset.bytes;
            }

            private void LoadTextAsync(Action<byte[]> callback)
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
                    if (setting.CryptoType == CryptoType.AES)
                    {
                        callback?.Invoke(CryptoUtils.Aes.DecryptBytesFromBytes(asset.bytes, setting.Password));
                    }
                    else
                    {
                        callback?.Invoke(asset.bytes);
                    }
                };
            }

            private void ReadRawData(byte[] bytes)
            {
                if (loaded)
                {
                    return;
                }

                loaded = true;
                stream.SetLength(0);
                stream.Write(bytes);
                allKeys.Clear();
                rowOffsets.Clear();
                stream.Seek(0, SeekOrigin.Begin);
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    rowOffsets.Add(reader.ReadInt64());
                }

                for (int i = 0; i < count; i++)
                {
                    stream.Seek(rowOffsets[i], SeekOrigin.Begin);
                    string key = reader.ReadString();
                    if (key.StartsWith("\0"))
                    {
                        stream.Seek(rowOffsets[i], SeekOrigin.Begin);
                        key = reader.ReadInt32().ToString();
                    }

                    allKeys.Add(key);
                }
            }

            private T ReadTableFromRawData<T>(string id) where T : class, IDataTable, new()
            {
                int index = allKeys.IndexOf(id);
                if (index >= 0)
                {
                    T table = new T();
                    stream.Seek(rowOffsets[index], SeekOrigin.Begin);
                    table.Read(reader);
                    dataTables.Add(id, table);
                    return table;
                }

                return null;
            }

            public override string ToString()
            {
                string result = $"{tableType}:(";
                bool first = true;
                foreach (string key in allKeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result += ",";
                    }

                    result += key;
                }

                result += ")";
                return result;
            }
        }
    }
}