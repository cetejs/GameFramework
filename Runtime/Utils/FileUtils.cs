using System;
using System.Collections.Generic;
using System.IO;

namespace GameFramework
{
    public static class FileUtils
    {
        public static void CheckDirectory(string path)
        {
            path = path.ReplaceSeparator().RemoveLastOf("/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        public static void ExistOrCreate(string path)
        {
            if (!File.Exists(path))
            {
                CheckDirectory(path);
                File.Create(path);
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

        public static bool CopyFile(string srcPath, string desPath, bool isOverwrite = false)
        {
            if (!File.Exists(srcPath))
            {
                return false;
            }

            CheckDirectory(desPath);
            File.Copy(srcPath, desPath, isOverwrite);
            return true;
        }

        public static string ReadAllText(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }

        public static async void ReadAllTextAsync(string path, Action<string> callback)
        {
            if (!File.Exists(path))
            {
                callback?.Invoke(null);
            }

            string text = await File.ReadAllTextAsync(path);
            callback?.Invoke(text);
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public static async void ReadAllBytesAsync(string path, Action<byte[]> callback)
        {
            if (!File.Exists(path))
            {
                callback?.Invoke(null);
            }

            byte[] bytes = await File.ReadAllBytesAsync(path);
            callback?.Invoke(bytes);
        }

        public static void WriteAllText(string path, string contents)
        {
            if (contents == null)
            {
                return;
            }

            CheckDirectory(path);
            File.WriteAllText(path, contents);
        }

        public static async void WriteAllTextAsync(string path, string contents, Action callback)
        {
            if (contents == null)
            {
                return;
            }

            CheckDirectory(path);
            await File.WriteAllTextAsync(path, contents);
            callback?.Invoke();
        }

        public static void WriteAllBytes(string path, byte[] contents)
        {
            if (contents == null)
            {
                return;
            }

            CheckDirectory(path);
            File.WriteAllBytes(path, contents);
        }

        public static async void WriteAllBytesAsync(string path, byte[] contents, Action callback)
        {
            if (contents == null)
            {
                return;
            }

            CheckDirectory(path);
            await File.WriteAllBytesAsync(path, contents);
            callback?.Invoke();
        }
    }
}