using System.Collections.Generic;
using System.IO;
using GameFramework.Generic;
using GameFramework.ObjectPoolService;
using GameFramework.Utils;
using UnityEngine;

namespace GameFramework
{
    public class ObjectPoolTest : MonoBehaviour
    {
        private ObjectPoolManager manager;
        private readonly List<EmptyObject> cubes = new List<EmptyObject>();

        private void AdjustConfig()
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/ObjectPoolConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("ObjectPoolConfig.asset", fullPath);
            }

            ObjectPoolConfig config = ObjectPoolConfig.Get();
            string samplesPath = EditorFileUtils.GetSamplesPath().Replace(Path.GetFullPath("."), "").RemoveFirstCount();
            string windowRootPath = Path.Combine(samplesPath, "ObjectPool/Prefabs/ObjectPool");
            AddressableUtils.CreateOrMoveEntry(windowRootPath, config.poolBundlePath);
#endif
        }

        private void Awake()
        {
            AdjustConfig();
        }

        private void Start()
        {
            manager = Global.GetService<ObjectPoolManager>();
        }

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = Screen.height / 50;
            if (GUILayout.Button("GetObject(Cube-Test/Cube)", style))
            {
                LifeTimeData data = ReferencePool.Get<LifeTimeData>();
                data.lifeTime = 10.0f;
                EmptyObject cube1 = manager.Get<EmptyObject>("Cube", transform);
                cube1.transform.position = Vector3.zero;
                EmptyObject cube2 = manager.Get<EmptyObject>("Test/Cube", transform);
                cube2.transform.position = Vector3.zero;
                cubes.Add(cube1);
                cubes.Add(cube2);
            }

            if (GUILayout.Button("Release(One)", style))
            {
                if (cubes.Count > 0)
                {
                    cubes.Pop().Release();
                }
            }

            if (GUILayout.Button("Release(Cube)", style))
            {
                manager.Release("Cube");
            }

            if (GUILayout.Button("ReleaseAll", style))
            {
                manager.ReleaseAll();
            }

            if (GUILayout.Button("Add(Test/Cube - 5)", style))
            {
                manager.Add("Test/Cube", 5);
            }

            if (GUILayout.Button("Remove(Test/Cube - 5)", style))
            {
                manager.Remove("Test/Cube", 5);
            }

            if (GUILayout.Button("Clear(Test/Cube)", style))
            {
                manager.Clear("Test/Cube");
            }

            if (GUILayout.Button("ClearAll", style))
            {
                manager.ClearAll();
            }
        }
    }
}