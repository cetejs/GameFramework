using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal class AssetBundleWindow : SubWindow
    {
        private BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;
        private BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        private Editor settingEditor;
        private MethodInfo getShaderVariantEntries;
        private ShaderVariantCollection excludeCollection;

        private Dictionary<string, List<string>> builds = new Dictionary<string, List<string>>();
        private HashSet<string> dependencies = new HashSet<string>();
        private HashSet<string> scenes = new HashSet<string>();
        private HashSet<string> materials = new HashSet<string>();
        private HashSet<string> shaders = new HashSet<string>();
        private HashSet<string> builtinResources = new HashSet<string>();
        private Dictionary<string, HashSet<string>> spriteAtlases = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, ShaderVariantData> shaderVariants = new Dictionary<string, ShaderVariantData>();
        private Dictionary<string, List<ShaderVariantCollection.ShaderVariant>> collectedShaderVariants = new Dictionary<string, List<ShaderVariantCollection.ShaderVariant>>();

        private readonly string[] BundleFilter = {".meta", ".cs", ".shader"};
        private readonly string[] TextureExtensions = new[] {".png"};
        private readonly string SceneExtension = ".unity";
        private readonly string MaterialExtension = ".mat";
        private readonly string ShaderVariantExtension = ".shadervariants";
        private readonly string SpriteAtlaslExtension = ".spriteatlas";

        private string BundleRoot
        {
            get { return PathUtils.Combine("Assets", AssetSetting.Instance.BundleAssetName); }
        }

        private string BuiltinResourcesRoot
        {
            get { return PathUtils.Combine(PathUtils.GetPackagePath(), "BuiltinResources"); }
        }

        private string ShaderVariantsPath
        {
            get { return StringUtils.Concat("Assets/", AssetSetting.Instance.ShaderVariantsAssetPath, ShaderVariantExtension); }
        }

        private string SpriteAtlasAssetPath
        {
            get { return PathUtils.Combine("Assets", AssetSetting.Instance.SpriteAtlasAssetPath); }
        }

        public override void Init(string name, GameWindow parent)
        {
            base.Init("AssetBundle", parent);
            settingEditor = Editor.CreateEditor(AssetSetting.Instance);
            buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            buildOptions = (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField("Build Options", buildOptions);
            buildTarget = (BuildTarget) EditorGUILayout.EnumPopup("Build Target", buildTarget);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Build"))
            {
                BuildAssetBundles();
            }

            if (GUILayout.Button("Clear"))
            {
                ClearAssetBundle();
            }

            EditorGUILayout.EndHorizontal();
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }

        private void BuildAssetBundles()
        {
            try
            {
                builds.Clear();
                dependencies.Clear();
                materials.Clear();
                builtinResources.Clear();
                string bundlePath = AssetSetting.Instance.BundleAssetPath;
                string outputPath = AssetSetting.Instance.BundleSavePath;
                List<FileInfo> filesInfos = new List<FileInfo>();
                FileUtils.GetFiles(bundlePath, filesInfos, BundleFilter);
                for (int i = 0; i < filesInfos.Count; i++)
                {
                    FileInfo fileInfo = filesInfos[i];
                    string fullName = fileInfo.FullName.ReplaceSeparator();
                    string relativePath = fullName.Substring(bundlePath.Length + 1);
                    string bundleName = StringUtils.Concat(relativePath.RemoveLastOf("/"));
                    string assetName = fullName.Substring(PathUtils.ProjectPath.Length + 1);
                    if (assetName.EndsWith(SceneExtension))
                    {
                        scenes.Add(assetName);
                        continue;
                    }

                    if (assetName.EndsWith(TextureExtensions))
                    {
                        AddTexture(bundleName, assetName);
                        continue;
                    }

                    AddBundleBuild(bundleName, assetName);
                    AddDependencies(bundleName, assetName);
                }

                AddScenes();
                AddShaders();
                AddBuiltinResources();
                AddSpriteAtlas();

                List<AssetBundleBuild> bundleBuilds = new List<AssetBundleBuild>();
                foreach (KeyValuePair<string, List<string>> build in builds)
                {
                    bundleBuilds.Add(new AssetBundleBuild()
                    {
                        assetBundleName = StringUtils.Concat(build.Key, ".", AssetSetting.Instance.BundleExtension),
                        assetNames = build.Value.ToArray()
                    });
                }

                if (bundleBuilds.Count > 0)
                {
                    DirectoryUtils.CreateDirectory(outputPath);
                    BuildPipeline.BuildAssetBundles(outputPath, bundleBuilds.ToArray(), buildOptions, buildTarget);
                }

                if (AssetSetting.Instance.DeleteShaderVariantsWhenBuild)
                {
                    AssetDatabase.DeleteAsset(ShaderVariantsPath);
                }

                if (AssetSetting.Instance.DeleteSpriteAtlasWhenBuild)
                {
                    DirectoryUtils.DeleteDirectory(SpriteAtlasAssetPath);
                    FileUtils.DeleteFile(StringUtils.Concat(SpriteAtlasAssetPath, ".meta"));
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
            }

            AssetDatabase.Refresh();
        }

        private void AddBundleBuild(string bundleName, string assetName)
        {
            if (!builds.TryGetValue(bundleName, out List<string> list))
            {
                list = new List<string>();
                builds.Add(bundleName, list);
            }

            list.Add(assetName);
        }

        private void AddDependencies(string bundleName, string assetName)
        {
            bundleName = StringUtils.Concat(bundleName, "_dp");
            string[] dps = AssetDatabase.GetDependencies(assetName, true);
            foreach (string dp in dps)
            {
                if (dp.EndsWith(MaterialExtension))
                {
                    materials.Add(dp);
                }

                if (dp == assetName)
                {
                    continue;
                }

                if (dp.StartsWith(BundleRoot))
                {
                    continue;
                }

                if (dp.EndsWith(BundleFilter))
                {
                    continue;
                }

                if (dp.EndsWith(TextureExtensions))
                {
                    AddTexture(null, dp);
                    continue;
                }

                if (dp.StartsWith(BuiltinResourcesRoot))
                {
                    builtinResources.Add(dp);
                    continue;
                }

                if (dependencies.Contains(dp))
                {
                    continue;
                }

                dependencies.Add(dp);
                AddBundleBuild(bundleName, dp);
            }
        }

        private void AddScenes()
        {
            if (scenes.Count == 0)
            {
                return;
            }

            foreach (string scene in scenes)
            {
                string bundleName = StringUtils.Concat(scene.GetLastOf("/").RemoveLastOf("."));
                string[] dps = AssetDatabase.GetDependencies(scene, true);
                foreach (string dp in dps)
                {
                    if (dp.EndsWith(".mat"))
                    {
                        materials.Add(dp);
                    }

                    if (dp.EndsWith(BundleFilter))
                    {
                        continue;
                    }

                    if (dp.StartsWith(BuiltinResourcesRoot))
                    {
                        builtinResources.Add(dp);
                    }
                }

                AddBundleBuild(bundleName, scene);
            }
        }

        private void AddShaders()
        {
            if (materials.Count == 0)
            {
                return;
            }

            string path = ShaderVariantsPath;
            if (File.Exists(path))
            {
                excludeCollection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
                excludeCollection.Clear();
            }
            else if (excludeCollection == null)
            {
                excludeCollection = new ShaderVariantCollection();
                FileUtils.CheckDirectory(path);
                AssetDatabase.CreateAsset(excludeCollection, path);
            }

            string bundleName = AssetSetting.Instance.ShaderBundleName;
            string[] filterKeywords = new string[] { };

            shaders.Clear();
            shaderVariants.Clear();
            collectedShaderVariants.Clear();
            foreach (string assetName in materials)
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetName);
                AddShaderVariants(mat, filterKeywords);
            }

            foreach (string shader in shaders)
            {
                AddBundleBuild(bundleName, shader);
            }

            excludeCollection.Clear();
            foreach (List<ShaderVariantCollection.ShaderVariant> variants in collectedShaderVariants.Values)
            {
                foreach (ShaderVariantCollection.ShaderVariant variant in variants)
                {
                    excludeCollection.Add(variant);
                }
            }

            AssetDatabase.SaveAssetIfDirty(excludeCollection);
            AddBundleBuild(bundleName, path);
        }

        private void AddBuiltinResources()
        {
            foreach (string assetName in builtinResources)
            {
                AddBundleBuild(AssetSetting.Instance.BuiltinResourcesBundleName, assetName);
            }
        }

        private void AddSpriteAtlas()
        {
            if (EditorSettings.spritePackerMode == SpritePackerMode.Disabled)
            {
                EditorSettings.spritePackerMode = SpritePackerMode.BuildTimeOnlyAtlas;
            }

            foreach (KeyValuePair<string, HashSet<string>> kvPair in spriteAtlases)
            {
                string bundleName = kvPair.Key;
                HashSet<string> assetNames = kvPair.Value;
                List<Object> sprites = new List<Object>();
                foreach (string assetName in assetNames)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetName);
                    if (sprite != null)
                    {
                        sprites.Add(sprite);
                    }

                    AddBundleBuild(bundleName, assetName);
                }

                if (sprites.Count > 0)
                {
                    DirectoryUtils.CreateDirectory(SpriteAtlasAssetPath);
                    SpriteAtlas atlas = new SpriteAtlas();
                    string path = StringUtils.Concat(SpriteAtlasAssetPath, "/", bundleName, SpriteAtlaslExtension);
                    atlas.SetPackingSettings(new SpriteAtlasPackingSettings()
                    {
                        padding = 2 << AssetSetting.Instance.SpriteAtlasPackingPadding,
                        enableRotation = false,
                        enableTightPacking = false,
                        enableAlphaDilation = false
                    });

                    atlas.Add(sprites.ToArray());
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.CreateAsset(atlas, path);
                    AddBundleBuild(bundleName, path);
                }
            }
        }

        private void AddTexture(string bundleName, string assetName)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                bundleName = assetName.RemoveLastOf("/").GetLastOf("/");
            }

            if (!spriteAtlases.TryGetValue(bundleName, out HashSet<string> hashSet))
            {
                hashSet = new HashSet<string>();
                spriteAtlases.Add(bundleName, hashSet);
            }

            hashSet.Add(assetName);
        }

        private void AddShaderVariants(Material mat, string[] filterKeywords)
        {
            Shader shader = mat.shader;
            shaders.Add(AssetDatabase.GetAssetPath(shader));
            string shaderName = shader.name;
            string[] keywords = mat.shaderKeywords;
            if (!shaderVariants.TryGetValue(shaderName, out ShaderVariantData data))
            {
                data = GetShaderVariantData(shader, excludeCollection, filterKeywords);
                shaderVariants.Add(shaderName, data);
            }

            if (!collectedShaderVariants.TryGetValue(shaderName, out List<ShaderVariantCollection.ShaderVariant> variants))
            {
                variants = new List<ShaderVariantCollection.ShaderVariant>();
                collectedShaderVariants.Add(shaderName, variants);
            }

            HashSet<int> passTypes = new HashSet<int>(data.passTypes);
            foreach (int passType in passTypes)
            {
                ShaderVariantCollection.ShaderVariant variant;
                try
                {
                    variant = new ShaderVariantCollection.ShaderVariant(shader, (PassType) passType, keywords);
                }
                catch
                {
                    variant = new ShaderVariantCollection.ShaderVariant(shader, (PassType) passType);
                }

                bool exist = false;
                foreach (ShaderVariantCollection.ShaderVariant temp in variants)
                {
                    if (temp.passType == variant.passType && temp.keywords.SequenceEqual(variant.keywords))
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    variants.Add(variant);
                }
            }
        }

        private ShaderVariantData GetShaderVariantData(Shader shader, ShaderVariantCollection svc, string[] filterKeywords, int maxEntries = 2048)
        {
            if (getShaderVariantEntries == null)
            {
                getShaderVariantEntries = typeof(ShaderUtil).GetMethod("GetShaderVariantEntriesFiltered", BindingFlags.NonPublic | BindingFlags.Static);
            }

            int[] passTypes = null;
            string[] keywords = null, remainingKeywords = null;
            object[] args = new object[] {shader, maxEntries, filterKeywords, svc, passTypes, keywords, remainingKeywords};
            getShaderVariantEntries.Invoke(null, args);

            return new ShaderVariantData()
            {
                shader = shader,
                passTypes = ((int[]) args[4]).ToList(),
                keywords = ((string[]) args[5]).ToList(),
                remainingKeywords = ((string[]) args[6]).ToList(),
            };
        }

        private void ClearAssetBundle()
        {
            DirectoryUtils.DeleteDirectory(AssetSetting.Instance.BundleSavePath);
            FileUtils.DeleteFile(StringUtils.Concat(AssetSetting.Instance.BundleSavePath, ".meta"));
            AssetDatabase.Refresh();
        }

        private class ShaderVariantData
        {
            public Shader shader;
            public List<int> passTypes;
            public List<string> keywords;
            public List<string> remainingKeywords;
        }
    }
}