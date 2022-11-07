using System.Collections.Generic;
using System.IO;
using GameFramework.Utils;

namespace GameFramework
{
    public class PackageInfo
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

        public static PackageInfo Get()
        {
            if (instance != null)
            {
                return instance;
            }

            string packageJsonPath = Path.Combine(EditorFileUtils.GetPackagePah(), "package.json");
            string packageJson = File.ReadAllText(packageJsonPath);
            instance = JsonUtils.ToObject<PackageInfo>(packageJson);
            return instance;
        }
    }
}