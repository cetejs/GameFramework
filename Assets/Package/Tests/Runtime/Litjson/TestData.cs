using System.Collections.Generic;
using UnityEngine;

 public class TestData
    {
        public bool item1;
        public int item2;
        public long item3;
        public float item4;
        public double item5;
        public string item6;

        public Vector2 v2;
        public Vector3 v3;
        public Vector4 v4;
        public Quaternion q;
        public Color c;
        public Color32 c32;

        public SubTestData[] array;
        public List<SubTestData> list;
        public Dictionary<string, SubTestData> dict;

        public static TestData NewTest()
        {
            TestData data = new TestData()
            {
                item1 = true,
                item2 = int.MaxValue,
                item3 = long.MaxValue,
                item4 = float.MaxValue,
                item5 = double.MaxValue * 0.5d,
                item6 = "123",
                v2 = new Vector2(1.0f, 2.0f),
                v3 = new Vector3(1.0f, 2.0f, 3.0f),
                v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f),
                q = Quaternion.Euler(1.0f, 2.0f, 3.0f),
                c = new Color(0.1f, 0.2f, 0.3f, 0.4f),
                c32 = new Color32(1, 2, 3, 4),
            };

            SubTestData data1 = new SubTestData()
            {
                item = 1
            };
        
            SubTestData data2 = new SubTestData()
            {
                item = 2
            };

            data.array = new[]
            {
                data1, data2
            };

            data.list = new List<SubTestData>()
            {
                data1,
                data2
            };

            data.dict = new Dictionary<string, SubTestData>()
            {
                {"1", data1},
                {"2", data2}
            };
            
            return data;
        }

        public override string ToString()
        {
            return $"{item1} {item2} {item3} {item4} {item5} {item6} {v2} {v3} {v4} {q} {c} {c32} {string.Join<SubTestData>(",", array)} {string.Join(",", list)} {string.Join(",", dict.Keys)} {string.Join(",", dict.Values)}";
        }
    }

    public class SubTestData
    {
        public int item;

        public override string ToString()
        {
            return item.ToString();
        }
    }