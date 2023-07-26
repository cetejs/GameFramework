using System.Collections.Generic;

namespace GameFramework
{
    internal class DefaultHandler : IPersistentHandler
    {
        private int storageId;
        private int suffixIndex;
        private string savePath;
        private Dictionary<string, string> data;

        public DefaultHandler(int storageId = 0)
        {
            this.storageId = storageId;
            suffixIndex = storageId.ToString().Length + 1;
            savePath = PersistentSetting.Instance.GetSavePath(this.storageId);
            string json = FileUtils.ReadAllText(savePath);
            if (!string.IsNullOrEmpty(json))
            {
                data = JsonUtils.ToObject<Dictionary<string, string>>(json);
            }
            else
            {
                data = new Dictionary<string, string>();
            }
        }

        public string GetString(string key, string defaultValue)
        {
            if (data.TryGetValue(GetKey(key), out string value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            data[GetKey(key)] = value;
        }

        public string[] GetAllKeys()
        {
            int i = 0;
            string[] results = new string[data.Count];
            foreach (string key in data.Keys)
            {
                results[i++] = key.Remove(key.Length - suffixIndex);
            }

            return results;
        }

        public void GetAllKeys(List<string> results)
        {
            results.Clear();
            if (results.Capacity < data.Count)
            {
                results.Capacity = data.Count;
            }

            foreach (string key in data.Keys)
            {
                results.Add(key.Remove(key.Length - suffixIndex));
            }
        }

        public bool HasKey(string key)
        {
            return data.ContainsKey(GetKey(key));
        }

        public void DeleteKey(string key)
        {
            data.Remove(GetKey(key));
        }

        public void DeleteAll()
        {
            data.Clear();
            FileUtils.DeleteFile(savePath);
        }

        public void Save()
        {
            FileUtils.WriteAllText(savePath, JsonUtils.ToJson(data));
        }

        private string GetKey(string key)
        {
            return StringUtils.Concat(key, "_", storageId);
        }
    }
}