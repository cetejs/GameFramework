using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class BuiltinResourcesProcessor : AssetModificationProcessor
    {
        private static readonly string BuiltinResourcesGuid = "0000000000000000f000000000000000";

        private static string BuiltinResourcesPath
        {
            get { return PathUtils.Combine(PathUtils.GetPackagePath(), "BuiltinResources"); }
        }

        private static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if (path.EndsWith(".unity"))
                {
                    Material skybox = RenderSettings.skybox;
                    if (skybox != null)
                    {
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(skybox, out string guid, out long localId);
                        if (guid == BuiltinResourcesGuid)
                        {
                            RenderSettings.skybox = LoadBuiltinMaterial("Default-Skybox");
                        }
                    }

                    Image[] images = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Image>();
                    foreach (Image image in images)
                    {
                        ReplaceBuiltinSprite(image);
                    }

                    Renderer[] renderers = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        ReplaceBuiltinMaterial(renderer);
                    }
                }
                else if (path.EndsWith(".prefab"))
                {
                    Image[] images = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Image>();
                    foreach (Image image in images)
                    {
                        ReplaceBuiltinSprite(image);
                    }

                    Renderer[] renderers = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        ReplaceBuiltinMaterial(renderer);
                    }

                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        images = prefab.GetComponentsInChildren<Image>(true);
                        foreach (Image image in images)
                        {
                            ReplaceBuiltinSprite(image);
                        }

                        renderers = prefab.GetComponentsInChildren<Renderer>(true);
                        foreach (Renderer renderer in renderers)
                        {
                            ReplaceBuiltinMaterial(renderer);
                        }
                    }
                }
            }

            return paths;
        }

        private static void ReplaceBuiltinSprite(Image image)
        {
            Sprite sprite = image.sprite;
            if (sprite != null)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localId);
                if (guid == BuiltinResourcesGuid)
                {
                    image.sprite = LoadBuiltinSprite(sprite.name);
                }
            }
        }

        private static void ReplaceBuiltinMaterial(Renderer renderer)
        {
            Material material = renderer.sharedMaterial;
            if (material != null)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(material, out string guid, out long localId);
                if (guid == BuiltinResourcesGuid)
                {
                    renderer.sharedMaterial = LoadBuiltinMaterial(material.name);
                }
            }
        }

        private static Material LoadBuiltinMaterial(string name)
        {
            string path = StringUtils.Concat(BuiltinResourcesPath, "/Materials/", name, ".mat");
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        private static Sprite LoadBuiltinSprite(string name)
        {
            string path = StringUtils.Concat(BuiltinResourcesPath, "/Textures/", name, ".png");
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}