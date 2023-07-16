using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    public static class PackageUtils
    {
        private static PackageInfo info;

        private static void ReadInfo()
        {
            if (info != null)
            {
                return;
            }

            string packageJsonPath = Path.Combine(PathUtils.GetPackageFullPath(), "package.json");
            string packageJson = File.ReadAllText(packageJsonPath);
            info = JsonUtility.FromJson<PackageInfo>(packageJson);
        }

        public static string GetVersion()
        {
            ReadInfo();
            return info.version;
        }

        private class PackageInfo
        {
            public string name;
            public string displayName;
            public string description;
            public string version;
            public string unity;
            public string license;
            public Dictionary<string, string> dependencies;
            public Dictionary<string, string> author;
            public List<Dictionary<string, string>> samples;

            private static PackageInfo instance;

            public static PackageInfo Instance
            {
                get
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    string packageJsonPath = Path.Combine(PathUtils.GetPackageFullPath(), "package.json");
                    string packageJson = File.ReadAllText(packageJsonPath);
                    instance = JsonUtility.FromJson<PackageInfo>(packageJson);
                    return instance;
                }
            }
        }
    }
}