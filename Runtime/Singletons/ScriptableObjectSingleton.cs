using UnityEngine;

namespace GameFramework
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string name = typeof(T).Name;
                    instance = Resources.Load<T>(name);
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<T>();
                        string path = $"Assets/Resources/{name}.asset";
                        FileUtils.CheckDirectory(path);
                        UnityEditor.AssetDatabase.CreateAsset(instance, path);
                        UnityEditor.AssetDatabase.SaveAssets();
                    }
#endif
                }

                return instance;
            }
        }
    }
}