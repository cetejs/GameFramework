using GameFramework.DataNodeService;
using GameFramework.Generic;
using NUnit.Framework;
using UnityEngine;

public class DataNodeTest
{
    [Test]
    public void Test()
    {
        Data<int> data1 = ReferencePool.Get<Data<int>>();
        data1.item = 1;
        Data<int, float> data2 = ReferencePool.Get<Data<int, float>>();
        data2.item1 = 2;
        data2.item2 = 2.0f;
        Data<int, float, string> data3 = ReferencePool.Get<Data<int, float, string>>();
        data3.item1 = 3;
        data3.item2 = 3.0f;
        data3.item3 = "3";
        DataNodeManager manager = Global.GetService<DataNodeManager>();
        manager.SetData("a/b/c", data3);
        manager.SetData("a/b", data2);
        manager.SetData("a", data1);
        Debug.Log("SetData(a/b/c)");
        Debug.Log("SetData(a/b)");
        Debug.Log("SetData(a)");

        data1 = manager.GetData<Data<int>>("a");
        data2 = manager.GetData<Data<int, float>>("a/b");
        data3 = manager.GetData<Data<int, float, string>>("a/b/c");
        Debug.Log($"GetData(a) = {data1}");
        Debug.Log($"GetData(a/b) = {data2}");
        Debug.Log($"GetData(a/b/c) = {data3}");

        manager.RemoveNode("a");
        Debug.Log("RemoveNode(a)");
        Debug.Log($"HasNode(a/b) = {manager.HasNode("a/b")}");
        Debug.Log($"HasNode(a/b/c) = {manager.HasNode("a/b/c")}");
    }
}