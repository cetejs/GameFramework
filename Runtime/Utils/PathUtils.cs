using System.IO;
using UnityEngine;

namespace GameFramework
{
    public static class PathUtils
    {
        public static string ProjectPath
        {
            get { return Path.GetFullPath("."); }
        }

        public static string AssetPath
        {
            get { return Application.dataPath; }
        }

        public static string StreamingAssetsPath
        {
            get { return Application.streamingAssetsPath; }
        }

        public static string PersistentDataPath
        {
            get { return Application.persistentDataPath; }
        }

        public static string GetPackagePath()
        {
            string package = "Packages/com.cetejs.gameframework";
            if (Directory.Exists(Path.GetFullPath(package)))
            {
                return package;
            }

            return "Assets/Package";
        }

        public static string GetSamplesPath()
        {
            string samples = Combine("Assets/Samples/Game Framework", PackageUtils.GetVersion());
            if (Directory.Exists(Path.GetFullPath(samples)))
            {
                return samples;
            }

            return "Assets/Samples";
        }

        public static string GetPackageFullPath()
        {
            string package = Path.GetFullPath("Packages/com.cetejs.gameframework");
            if (Directory.Exists(package))
            {
                return package;
            }

            return Path.GetFullPath("Assets/Package");
        }

        public static string GetSamplesFullPath()
        {
            string samples = Path.GetFullPath(Combine("Assets/Samples/Game Framework", PackageUtils.GetVersion()));
            if (Directory.Exists(samples))
            {
                return samples;
            }

            return Path.GetFullPath("Assets/Samples");
        }

        public static string Combine(string path1, string path2)
        {
            return StringUtils.Join("/", path1, path2);
        }

        public static string Combine(string path1, string path2, string path3)
        {
            return StringUtils.Join("/", path1, path2, path3);
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return StringUtils.Join("/", path1, path2, path3, path4);
        }

        public static string Combine(params string[] paths)
        {
            return StringUtils.Join("/", paths);
        }
    }
}