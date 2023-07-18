using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework
{
    internal class SceneLoader
    {
        public void LoadScene(string name, LoadSceneMode mode)
        {
            AssetManager.Instance.LoadBundle(name);
            SceneManager.LoadScene(name, mode);
        }

        public SceneAsyncOperation LoadSceneAsync(string name, LoadSceneMode mode)
        {
            SceneAsyncOperation loadOperation = new SceneAsyncOperation();
            AssetAsyncOperation bundleOperation = AssetManager.Instance.LoadBundleAsync(name);
            if (bundleOperation != null)
            {
                loadOperation.SetDependency(bundleOperation);
                bundleOperation.OnCompleted += _ =>
                {
                    InternalLoadSceneAsync(name, mode, loadOperation);
                };
            }
            else
            {
                InternalLoadSceneAsync(name, mode, loadOperation);
            }

            return loadOperation;
        }

        public SceneAsyncOperation UnloadSceneAsync(string name, bool unloadBundle)
        {
            SceneAsyncOperation unloadOperation = new SceneAsyncOperation();
            if (unloadBundle)
            {
                AssetAsyncOperation bundleOperation = AssetManager.Instance.UnloadBundleAsync(name, true);
                if (bundleOperation != null)
                {
                    unloadOperation.SetDependency(bundleOperation);
                    bundleOperation.OnCompleted += _ =>
                    {
                        InternalUnloadSceneAsync(name, unloadOperation);
                    };
                }
                else
                {
                    InternalUnloadSceneAsync(name, unloadOperation);
                }
            }
            else
            {
                InternalUnloadSceneAsync(name, unloadOperation);
            }

            return unloadOperation;
        }

        private void InternalLoadSceneAsync(string name, LoadSceneMode mode, SceneAsyncOperation loadOperation)
        {
            AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(name, mode);
            loadOperation.SetOperation(sceneOperation);
            sceneOperation.completed += _ =>
            {
                loadOperation.Completed(null);
            };
        }

        private void InternalUnloadSceneAsync(string name, SceneAsyncOperation unloadOperation)
        {
            Scene scene = SceneManager.GetSceneByName(name);
            if (!scene.isLoaded)
            {
                unloadOperation.Completed(null);
                return;
            }

            AsyncOperation sceneOperation = SceneManager.UnloadSceneAsync(name);
            if (sceneOperation == null)
            {
                unloadOperation.Completed(null);
                return;
            }

            unloadOperation.SetOperation(sceneOperation);
            sceneOperation.completed += _ =>
            {
                unloadOperation.Completed(null);
            };
        }
    }
}