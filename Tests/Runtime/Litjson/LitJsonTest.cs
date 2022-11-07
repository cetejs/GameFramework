using LitJson;
using NUnit.Framework;
using UnityEngine;

public class LitJsonTest
{
    [Test]
    public void Test()
    {
        TestData data = TestData.NewTest();
        string json = JsonMapper.ToJson(data);
        Debug.Log(json);
        data = JsonMapper.ToObject<TestData>(json);
        Debug.Log(data.ToString());
    }
}