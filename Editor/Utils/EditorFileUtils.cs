using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public static class EditorFileUtils
    {
        public static string GetPackagePah()
        {
            string package = Path.GetFullPath("Packages/com.cetejs.gameframework");
            if (Directory.Exists(package))
            {
                return package;
            }

            return Path.GetFullPath("Assets/Package");
        }

        public static string GetSamplesPath()
        {
            PackageInfo info = PackageInfo.Get();
            string samples = Path.GetFullPath($"Assets/Samples/Game Framework/{info.version}");
            if (Directory.Exists(samples))
            {
                return samples;
            }

            return Path.GetFullPath("Assets/Samples");
        }

        public static void CopyAsset(string srcName, string desPath)
        {
            string package = GetPackagePah();
            string fullPath = Path.Combine(package, "Content", srcName);

            if (SafeCopyFile(fullPath, desPath, true))
            {
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"Dont exist asset {fullPath}");
            }
        }

        private static bool SafeCopyFile(string srcPath, string desPath, bool isOverwrite = false)
        {
            if (!File.Exists(srcPath))
            {
                return false;
            }

            CheckDirectory(desPath);
            File.Copy(srcPath, desPath, isOverwrite);
            return true;
        }

        public static void CheckDirectory(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }
    }
}