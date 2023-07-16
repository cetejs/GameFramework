using System.Collections.Generic;
using System.IO;

namespace GameFramework
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

        public static void GetFiles(string path, List<FileInfo> fileInfos, params string[] ignoreFilters)
        {
            fileInfos.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            GetFiles(directoryInfo, fileInfos, ignoreFilters);
        }

        private static void GetFiles(DirectoryInfo directoryInfo, List<FileInfo> fileInfos, params string[] ignoreFilters)
        {
            if ((directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                return;
            }

            if (directoryInfo.Name.StartsWith("."))
            {
                return;
            }

            fileInfos.AddRange(directoryInfo.GetFiles());
            fileInfos.RemoveAll(fileInfo => fileInfo.FullName.EndsWith(ignoreFilters));
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            foreach (DirectoryInfo tempInfo in directoryInfos)
            {
                GetFiles(tempInfo, fileInfos, ignoreFilters);
            }
        }

        public static string GetExtension(string path, bool isIgnoreCase = true)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path.RemoveLastOf("/"));
            FileInfo[] filesInfo = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            string fileName = path.GetLastOf("/");
            if (isIgnoreCase)
            {
                fileName = fileName.ToLower();
            }

            foreach (FileInfo fileInfo in filesInfo)
            {
                string fullName = fileInfo.FullName.ReplaceSeparator();
                if (!fullName.EndsWith(".meta"))
                {
                    string filePath = fileInfo.Name;
                    if (isIgnoreCase)
                    {
                        filePath = fileName.ToLower();
                    }

                    if (filePath.StartsWith(fileName))
                    {
                        return fileInfo.Extension;
                    }
                }
            }

            return string.Empty;
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void WriteAllText(string path, string contents)
        {
            CheckDirectory(path);
            File.WriteAllText(path, contents);
        }
    }
}