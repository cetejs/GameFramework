using System.Collections.Generic;
using GameFramework.Generic;
using GameFramework.Persistent;
using NUnit.Framework;
using UnityEngine;

public class PersistentTest
{
    [Test]
    public void TestSetData()
    {
        TestData data = TestData.NewTest();
        PersistentManager manager = Global.GetService<PersistentManager>();
        manager.SetData("TestData", data);
        manager.SetData("TestData.item1", data.item1);
        manager.SetData("TestData.item2", data.item2);
        manager.SetData("TestData.item3", data.item3);
        manager.SetData("TestData.item4", data.item4);
        manager.SetData("TestData.item5", data.item5);
        manager.SetData("TestData.item6", data.item6);
        manager.SetData("TestData.v2", data.v2);
        manager.SetData("TestData.v3", data.v3);
        manager.SetData("TestData.v4", data.v4);
        manager.SetData("TestData.q", data.q);
        manager.SetData("TestData.c", data.c);
        manager.SetData("TestData.c32", data.c32);
        manager.SetData("TestData.array", data.array);
        manager.SetData("TestData.list", data.list);
        manager.SetData("TestData.dict", data.dict);

        Debug.Log(manager.GetData<TestData>("TestData"));
        Debug.Log(manager.GetData<bool>("TestData.item1"));
        Debug.Log(manager.GetData<int>("TestData.item2"));
        Debug.Log(manager.GetData<long>("TestData.item3"));
        Debug.Log(manager.GetData<float>("TestData.item4"));
        Debug.Log(manager.GetData<double>("TestData.item5"));
        Debug.Log(manager.GetData<string>("TestData.item6"));
        Debug.Log(manager.GetData<Vector2>("TestData.v2"));
        Debug.Log(manager.GetData<Vector3>("TestData.v3"));
        Debug.Log(manager.GetData<Vector4>("TestData.v4"));
        Debug.Log(manager.GetData<Quaternion>("TestData.q"));
        Debug.Log(manager.GetData<Color>("TestData.c"));
        Debug.Log(manager.GetData<Color32>("TestData.c32"));
        Debug.Log(string.Join<SubTestData>(",", manager.GetData<SubTestData[]>("TestData.array")));
        Debug.Log(string.Join(",", manager.GetData<List<SubTestData>>("TestData.list")));
        Debug.Log(string.Join(",", manager.GetData<Dictionary<string, SubTestData>>("TestData.dict")));
    }

    [Test]
    public void TestDeleteData()
    {
        PersistentManager manager = Global.GetService<PersistentManager>();
        manager.SetData("1", 1);
        Debug.Log($"TestDataKey is {manager.HasKey("TestData")}");
        manager.DeleteKey("TestData");
        string[] allKeys = manager.GetAllKeys();
        for (int i = 0; i < allKeys.Length; i++)
        {
            Debug.Log($"{allKeys[i]} {manager.GetString(allKeys[i])}");
        }
        Debug.Log($"TestDataKey is {manager.HasKey("TestData")}");
        manager.DeleteNodeKey("TestData");
        
        allKeys = manager.GetAllKeys();
        for (int i = 0; i < allKeys.Length; i++)
        {
            Debug.Log($"{allKeys[i]} {manager.GetString(allKeys[i])}");
        }
        
        manager.DeleteAll();
    }
    
    [Test]
    public void TestImportData()
    {
        PersistentManager manager = Global.GetService<PersistentManager>();
        string testData = manager.ExportData("TestData");
        Debug.Log(testData);
        manager.DeleteKey("TestData");
        manager.ImportData(testData);
        
        string nodeData = manager.ExportNodeData("TestData");
        Debug.Log(nodeData);
        manager.DeleteNodeKey("TestData");
        manager.ImportData(nodeData);
        
        string allData = manager.ExportAllData();
        Debug.Log(allData);
        manager.DeleteAll();
        manager.ImportData(allData);
    }
}