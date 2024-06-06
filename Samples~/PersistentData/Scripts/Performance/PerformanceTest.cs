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
            TestData1();
            TestData2();
            TestData3();
            TestData4();
        }

        private void TestData1()
        {
            string jsonStorageName1 = $"PerformanceJsonStorage1_{testCount}";
            string binaryStorageName1 = $"PerformanceBinaryStorage1_{testCount}";
            string dataKey = "data";
            PersistentManager.Instance.Delete(jsonStorageName1);
            PersistentManager.Instance.Delete(binaryStorageName1);

            List<PerformanceData1> data1 = new List<PerformanceData1>();
            for (int i = 0; i < testCount; i++)
            {
                data1.Add(new PerformanceData1()
                {
                    t1 = new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    },
                    t2 = new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    },
                    t3 = new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    },
                    t4 = new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    },
                    t5 = new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    }
                });
            }

            PersistentSetting.Instance.StorageMode = StorageMode.Json;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PersistentManager.Instance.SetData(jsonStorageName1, dataKey, data1);
            stopwatch.Stop();
            Debug.Log($"[Json TestData1] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(jsonStorageName1);
            stopwatch.Stop();
            Debug.Log($"[Json TestData1] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(jsonStorageName1);
            stopwatch.Stop();
            Debug.Log($"[Json TestData1] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData1> jsonGetData1 = PersistentManager.Instance.GetData<List<PerformanceData1>>(jsonStorageName1, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Json TestData1] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            PersistentSetting.Instance.StorageMode = StorageMode.Binary;
            stopwatch.Restart();
            PersistentManager.Instance.SetData(binaryStorageName1, dataKey, data1);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData1] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(binaryStorageName1);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData1] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(binaryStorageName1);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData1] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData1> binaryGetData1 = PersistentManager.Instance.GetData<List<PerformanceData1>>(binaryStorageName1, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData1] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            Debug.Log("------------------------------------------------------------------------------");
        }

        private void TestData2()
        {
            string jsonStorageName2 = $"PerformanceJsonStorage2_{testCount}";
            string binaryStorageName2 = $"PerformanceBinaryStorage2_{testCount}";
            string dataKey = "data";
            PersistentManager.Instance.Delete(jsonStorageName2);
            PersistentManager.Instance.Delete(binaryStorageName2);

            List<PerformanceData2> data2 = new List<PerformanceData2>();
            for (int i = 0; i < testCount; i++)
            {
                PerformanceData2 data = new PerformanceData2();
                data.list = new List<TransformData>();
                for (int j = 0; j < 5; j++)
                {
                    data.list.Add(new TransformData()
                    {
                        position = new CustomVector3(Random.insideUnitSphere * 1000f),
                        eulerAngles = new CustomVector3(Random.insideUnitSphere * 360f),
                        localScale = new CustomVector3(Random.insideUnitSphere * 100)
                    });
                }

                data2.Add(data);
            }

            PersistentSetting.Instance.StorageMode = StorageMode.Json;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PersistentManager.Instance.SetData(jsonStorageName2, dataKey, data2);
            stopwatch.Stop();
            Debug.Log($"[Json TestData2] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(jsonStorageName2);
            stopwatch.Stop();
            Debug.Log($"[Json TestData2] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(jsonStorageName2);
            stopwatch.Stop();
            Debug.Log($"[Json TestData2] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData2> jsonGetData2 = PersistentManager.Instance.GetData<List<PerformanceData2>>(jsonStorageName2, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Json TestData2] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            PersistentSetting.Instance.StorageMode = StorageMode.Binary;
            stopwatch.Restart();
            PersistentManager.Instance.SetData(binaryStorageName2, dataKey, data2);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData2] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(binaryStorageName2);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData2] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(binaryStorageName2);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData2] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData2> binaryGetData2 = PersistentManager.Instance.GetData<List<PerformanceData2>>(binaryStorageName2, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData2] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            Debug.Log("------------------------------------------------------------------------------");
        }

        private void TestData3()
        {
            string jsonStorageName3 = $"PerformanceJsonStorage3_{testCount}";
            string binaryStorageName3 = $"PerformanceBinaryStorage3_{testCount}";
            string dataKey = "data";
            PersistentManager.Instance.Delete(jsonStorageName3);
            PersistentManager.Instance.Delete(binaryStorageName3);

            List<PerformanceData3> data3 = new List<PerformanceData3>();
            for (int i = 0; i < testCount; i++)
            {
                data3.Add(new PerformanceData3()
                {
                    i1 = 1,
                    i2 = 2,
                    i3 = 3,
                    i4 = 4,
                    i5 = 5,
                });
            }

            PersistentSetting.Instance.StorageMode = StorageMode.Json;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PersistentManager.Instance.SetData(jsonStorageName3, dataKey, data3);
            stopwatch.Stop();
            Debug.Log($"[Json TestData3] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(jsonStorageName3);
            stopwatch.Stop();
            Debug.Log($"[Json TestData3] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(jsonStorageName3);
            stopwatch.Stop();
            Debug.Log($"[Json TestData3] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData3> jsonGetData3 = PersistentManager.Instance.GetData<List<PerformanceData3>>(jsonStorageName3, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Json TestData3] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            PersistentSetting.Instance.StorageMode = StorageMode.Binary;
            stopwatch.Restart();
            PersistentManager.Instance.SetData(binaryStorageName3, dataKey, data3);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData3] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(binaryStorageName3);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData3] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(binaryStorageName3);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData3] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData3> binaryGetData3 = PersistentManager.Instance.GetData<List<PerformanceData3>>(binaryStorageName3, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData3] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            Debug.Log("------------------------------------------------------------------------------");
        }

        private void TestData4()
        {
            string jsonStorageName4 = $"PerformanceJsonStorage4_{testCount}";
            string binaryStorageName4 = $"PerformanceBinaryStorage4_{testCount}";
            string dataKey = "data";
            PersistentManager.Instance.Delete(jsonStorageName4);
            PersistentManager.Instance.Delete(binaryStorageName4);

            List<PerformanceData4> data4 = new List<PerformanceData4>();
            for (int i = 0; i < testCount; i++)
            {
                PerformanceData4 data = new PerformanceData4();
                data.list = new List<int>();
                for (int j = 0; j < 5; j++)
                {
                    data.list.Add(j);
                }

                data4.Add(data);
            }

            PersistentSetting.Instance.StorageMode = StorageMode.Json;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PersistentManager.Instance.SetData(jsonStorageName4, dataKey, data4);
            stopwatch.Stop();
            Debug.Log($"[Json TestData4] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(jsonStorageName4);
            stopwatch.Stop();
            Debug.Log($"[Json TestData4] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(jsonStorageName4);
            stopwatch.Stop();
            Debug.Log($"[Json TestData4] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData4> jsonGetData4 = PersistentManager.Instance.GetData<List<PerformanceData4>>(jsonStorageName4, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Json TestData4] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            PersistentSetting.Instance.StorageMode = StorageMode.Binary;
            stopwatch.Restart();
            PersistentManager.Instance.SetData(binaryStorageName4, dataKey, data4);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData4] => SetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Save(binaryStorageName4);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData4] => Save : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            PersistentManager.Instance.Load(binaryStorageName4);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData4] => Load : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            List<PerformanceData4> binaryGetData4 = PersistentManager.Instance.GetData<List<PerformanceData4>>(binaryStorageName4, dataKey);
            stopwatch.Stop();
            Debug.Log($"[Binary TestData4] => GetData : {testCount} Milliseconds : {stopwatch.ElapsedMilliseconds}");

            Debug.Log("------------------------------------------------------------------------------");
        }
    }

    [Serializable]
    public struct PerformanceData1
    {
        public TransformData t1;
        public TransformData t2;
        public TransformData t3;
        public TransformData t4;
        public TransformData t5;
    }

    [Serializable]
    public class PerformanceData2
    {
        public List<TransformData> list;
    }

    [Serializable]
    public struct PerformanceData3
    {
        public int i1;
        public int i2;
        public int i3;
        public int i4;
        public int i5;
    }

    [Serializable]
    public class PerformanceData4
    {
        public List<int> list;
    }
}