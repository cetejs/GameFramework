using System;
using System.Text;
using GameFramework.Helper;

namespace GameFramework.Utils
{
    public static class StringUtils
    {
        public static string GetFirstOf(this string text, string value)
        {
            int index = text.IndexOf(value, StringComparison.Ordinal);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(0, index);
        }

        public static string GetLastOf(this string text, string value)
        {
            int index = text.LastIndexOf(value, StringComparison.Ordinal);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(index + 1);
        }

        public static string RemoveFirstOf(this string text, string value)
        {
            int index = text.IndexOf(value, StringComparison.Ordinal);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(index + value.Length);
        }

        public static string RemoveLastOf(this string text, string value)
        {
            int index = text.LastIndexOf(value, StringComparison.Ordinal);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(0, index);
        }

        public static string RemoveFirstCount(this string text, int count = 1)
        {
            if (text.Length < count)
            {
                return text;
            }

            return text.Remove(0, count);
        }

        public static string RemoveLastCount(this string text, int count = 1)
        {
            if (text.Length < count)
            {
                return text;
            }

            return text.Remove(text.Length - count, count);
        }

        public static string InitialsToLower(this string text)
        {
            if (text.Length < 1)
            {
                return text;
            }

            return string.Concat(text.Substring(0, 1).ToLower(), text.Substring(1));
        }

        public static string InitialsToUpper(this string text)
        {
            if (text.Length < 1)
            {
                return text;
            }

            return string.Concat(text.Substring(0, 1).ToUpper(), text.Substring(1));
        }

        public static string ReplaceNewline(this string text)
        {
            return text.Replace("\r\n", "\n");
        }

        public static string ReplaceSeparator(this string text)
        {
            return text.Replace("\\", "/");
        }

        public static int SearchOfBm(this string text, string pattern, bool isIgnoreCase = true)
        {
            return StringSearchHelper.SearchOfBm(text, pattern, isIgnoreCase);
        }

        public static void RemoveFirstCount(this StringBuilder text, int count = 1)
        {
            if (text.Length < count)
            {
                return;
            }

            text.Remove(0, count);
        }

        public static void RemoveLastCount(this StringBuilder text, int count = 1)
        {
            if (text.Length < count)
            {
                return;
            }

            text.Remove(text.Length - count, count);
        }

        public static void ReplaceNewline(this StringBuilder text)
        {
            text.Replace("\r\n", "\n");
        }
    }
}