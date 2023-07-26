namespace GameFramework
{
    internal interface IPersistentStorage : IPersistentHandler
    {
        void SetHandler(IPersistentHandler header);
        
        T GetData<T>(string key, T defaultValue);

        void SetData<T>(string key, T value);

        void DeleteNodeKey(string key);

        void ImportData(string json);

        string ExportData(string key);
        
        string ExportNodeData(string key);
        
        string ExportAllData();
    }
}