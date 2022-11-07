using System.IO;

namespace GameFramework.Utils
{
    public static class FileUtils
    {
        public static void CheckDirectory(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }

        public static void ExistOrCreate(string path)
        {
            if (File.Exists(path))
            {
                return;
            }

            CheckDirectory(path);
            File.Create(path);
        }
    }
}