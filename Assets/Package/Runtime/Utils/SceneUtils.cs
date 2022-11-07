using UnityEngine.SceneManagement;

namespace GameFramework.Utils
{
    public static class SceneUtils
    {
        public static bool IsSceneLoaded(string sceneName)
        {
            return IsSceneLoaded(SceneManager.GetSceneByName(sceneName));
        }

        public static bool IsSceneLoaded(Scene scene)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i) == scene)
                {
                    return true;
                }
            }

            return false;
        }
    }
}