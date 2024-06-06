using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace GameFramework.Samples.PersistentData
{
    public class PerformanceTest : MonoBehaviour
    {
        public int testCount = 1000;

        private void OnEnable()
        {
            string jsonStorageName = "PerformanceJsonStorage";
            string binaryStorageName = "PerformanceBinaryStorage";
            string dataListKey = "dataList";

            Stopwatch stopwatch = new Stopwatch();
            List<TransformData> dataList = new List<TransformData>(testCount);
            for (int i = 0; i < testCount; i++)
            {
                dataList.Add(new TransformData()
                {
                    position = new CustomVector3(Random.insideUnitSphere * 1000f),
                    eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                    localScale = new CustomVector3(Random.insideUnitSphere * 100)
                });
            }

            PersistentManager.Instance.Delete(jsonStorageName);
            PersistentManager.Instance.Delete(binaryStorageName);
            
            PersistentSetting.Instance.StorageMode = StorageMode.Json;
            stopwatch.Start();
            PersistentManager.Instance.SetData(jsonStorageName, dataListKey, dataList);
            stopwatch.Stop();
            Debug.Log($"[Json] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(jsonStorageName);
            stopwatch.Stop();
            Debug.Log($"[Json] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");
            
            stopwatch.Restart();
            PersistentManager.Instance.Load(jsonStorageName);
            stopwatch.Stop();
            Debug.Log($"[Json] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            PersistentSetting.Instance.StorageMode = StorageMode.Binary;
            stopwatch.Restart();
            PersistentManager.Instance.SetData(binaryStorageName, dataListKey, dataList);
            stopwatch.Stop();
            Debug.Log($"[Binary] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(binaryStorageName);
            stopwatch.Stop();
            Debug.Log($"[Binary] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");
            
            stopwatch.Restart();
            PersistentManager.Instance.Load(binaryStorageName);
            stopwatch.Stop();
            Debug.Log($"[Binary] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            Debug.Log("------------------------------------------------------------------------------");
        }
    }
}