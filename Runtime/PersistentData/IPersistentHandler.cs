using System.Collections.Generic;

namespace GameFramework
{
    internal interface IPersistentHandler
    {
        string GetString(string key, string defaultValue);

        void SetString(string key, string value);

        string[] GetAllKeys();
        
        void GetAllKeys(List<string> results);

        bool HasKey(string key);

        void DeleteKey(string key);

        void DeleteAll();

        void Save();
    }
}

