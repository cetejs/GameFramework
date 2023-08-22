using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace GameFramework
{
    public class LoadSceneTask : IStageTask
    {
        private int currentIndex;
        private string taskInfo;
        private SceneAsyncOperation operation;
        private List<SceneConfig> sceneConfigs;

        public float Progress
        {
            get
            {
                if (sceneConfigs.Count > 0)
                {
                    if (currentIndex >= sceneConfigs.Count)
                    {
                        return 1f;
                    }

                    return currentIndex / (float) sceneConfigs.Count + operation.Progress;
                }

                return 0f;
            }
        }

        public string TaskInfo
        {
            get
            {
                if (sceneConfigs.Count > 0)
                {
                    return StringUtils.Concat("LoadScene ", taskInfo);
                }

                return null;
            }
        }

        public LoadSceneTask(string sceneName, LoadSceneMode mode)
        {
            sceneConfigs = new List<SceneConfig>
            {
                new SceneConfig()
                {
                    SceneName = sceneName,
                    Mode = mode
                }
            };
        }

        public LoadSceneTask(List<SceneConfig> sceneName)
        {
            sceneConfigs = sceneName;
        }

        public IEnumerator Run()
        {
            for (currentIndex = 0; currentIndex < sceneConfigs.Count; currentIndex++)
            {
                SceneConfig config = sceneConfigs[currentIndex];
                operation = AssetManager.Instance.LoadSceneAsync(config.SceneName, config.Mode);
                taskInfo = config.SceneName;
                yield return operation;
            }
        }
    }

    public struct SceneConfig
    {
        public string SceneName;
        public LoadSceneMode Mode;
    }
}