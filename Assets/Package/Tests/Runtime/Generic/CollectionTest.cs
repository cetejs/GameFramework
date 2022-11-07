using System.Collections.Generic;
using GameFramework.Generic;
using NUnit.Framework;
using UnityEngine;

public class CollectionTest
{
    [Test]
    public void TestHeap()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            list.Add(Random.Range(0, 100));
        }

        Debug.Log($"Origin list : {string.Join(", ", list)}");
        Heap<int> heap = new Heap<int>(list);
        Debug.Log($"Build heap : {string.Join(", ", heap)}");
        heap.Sort();
        Debug.Log($"Sort heap : {string.Join(", ", heap)}");
        int value = Random.Range(0, 100);
        heap.Add(value);
        Debug.Log($"Add {value} heap : {string.Join(", ", heap)}");

        while (heap.Count > 0)
        {
            value = heap.ExtractMax();
            Debug.Log($"Extract max {value} heap : {string.Join(", ", heap)}");
        }
    }
    
    [Test]
    public void TestPriorityQueue()
    {
        PriorityQueue<int> queue = new PriorityQueue<int>();

        for (int i = 0; i < 10; i++)
        {
            queue.Enqueue(Random.Range(0, 100));
        }

        while (queue.Count > 0)
        {
            Debug.Log($"Dequeue priority queue {queue.Dequeue()}");
        }
    }
}
