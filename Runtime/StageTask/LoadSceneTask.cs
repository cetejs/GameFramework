using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace GameFramework
{
    public class LoadSceneTask : IStageTask
    {
        private int currentIndex;
        private string taskInfo;
        private string overrideInfo;
        private SceneAsyncOperation operation;
        private List<SceneConfig> sceneConfigs;

        public float Progress
        {
            get
            {
                if (currentIndex >= sceneConfigs.Count)
                {
                    return 1f;
                }

                if (sceneConfigs.Count > 0 && operation != null)
                {
                    return (currentIndex + operation.Progress) / sceneConfigs.Count;
                }

                return 0f;
            }
        }

        public string TaskInfo
        {
            get
            {
                if (overrideInfo != null)
                {
                    return overrideInfo;
                }

                return StringUtils.Concat("LoadScene ", taskInfo);
            }
        }

        public LoadSceneTask(string sceneName, LoadSceneMode mode, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            sceneConfigs = new List<SceneConfig>
            {
                new SceneConfig()
                {
                    SceneName = sceneName,
                    Mode = mode
                }
            };
        }

        public LoadSceneTask(List<SceneConfig> sceneConfigs, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            this.sceneConfigs = sceneConfigs;
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