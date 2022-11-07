using System.Collections;
using GameFramework.EventPoolService;
using GameFramework.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class EventPoolTest
{
    [UnityTest]
    public IEnumerator UnityTest()
    {
        EventManager manager = Global.GetService<EventManager>();
        manager.Register(1, OnEventTrigger);
        manager.Register(2, body =>
        {
            Debug.Log($"OnEventTrigger {body.Id}");
            Debug.Log($"Data {body.GetData()}");
            body.Unregister();
        });

        Debug.Log($"EventCount = {manager.EventCount} HandlerCount = {manager.HandlerCount(1)} HandlerCount = {manager.HandlerCount(2)}");

        Data<int> data1 = ReferencePool.Get<Data<int>>();
        data1.item = 2;
        Data<int, float> data2 = ReferencePool.Get<Data<int, float>>();
        data2.item1 = 3;
        data2.item2 = 4.0f;
        manager.Send(1);
        manager.Send(2, data2);
        manager.SendAsync(1, data1);

        Debug.Log($"EventCount = {manager.EventCount} HandlerCount1 = {manager.HandlerCount(1)} HandlerCount2 = {manager.HandlerCount(2)}");

        yield return null;
        manager.Unregister(1, OnEventTrigger);

        Debug.Log($"EventCount = {manager.EventCount} HandlerCount = {manager.HandlerCount(1)} HandlerCount2 = {manager.HandlerCount(2)}");

        manager.Send(1);
        manager.SendAsync(1);
        yield return null;
    }

    private void OnEventTrigger(EventBody body)
    {
        Debug.Log($"OnEventTrigger {body.Id}");
        Debug.Log($"Data {body.GetData()}");
    }
}