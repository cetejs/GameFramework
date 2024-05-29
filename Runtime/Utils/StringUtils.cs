using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework
{
    public static class StringUtils
    {
        [ThreadStatic]
        private static StringBuilder stringBuilder;
        private const int StringBuilderCapacity = 1024;

        private static void CheckStringBuilder()
        {
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder(StringBuilderCapacity);
            }
        }

        #region Concat

        public static string Concat(object arg0, object arg1)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0}{1}", arg0, arg1);
            return stringBuilder.ToString();
        }

        public static string Concat(object arg0, object arg1, object arg2)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0}{1}{2}", arg0, arg1, arg2);
            return stringBuilder.ToString();
        }

        public static string Concat(object arg0, object arg1, object arg2, object arg3)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0}{1}{2}{3}", arg0, arg1, arg2, arg3);
            return stringBuilder.ToString();
        }

        public static string Concat(params object[] args)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            foreach (object arg in args)
            {
                stringBuilder.Append(arg);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Format

        public static string Format(string format, object arg0)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat(format, arg0);
            return stringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat(format, arg0, arg1);
            return stringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return stringBuilder.ToString();
        }

        public static string Format(string format, params object[] args)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            stringBuilder.AppendFormat(format, args);
            return stringBuilder.ToString();
        }

        #endregion

        #region Join

        public static string Join(string separator, params string[] values)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            bool firstAppend = true;
            foreach (string value in values)
            {
                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        public static string Join(string separator, params object[] values)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            bool firstAppend = true;
            foreach (object value in values)
            {
                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        public static string Join(string separator, IEnumerable<string> values)
        {
            return Join<string>(separator, values);
        }

        public static string Join<T>(string separator, IEnumerable<T> values)
        {
            CheckStringBuilder();
            stringBuilder.Clear();
            bool firstAppend = true;
            foreach (T value in values)
            {
                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Text

        public static string GetFirstOf(this string text, string value)
        {
            int index = text.IndexOf(value);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(0, index);
        }

        public static string GetLastOf(this string text, string value)
        {
            int index = text.LastIndexOf(value);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(index + 1);
        }

        public static string RemoveFirstOf(this string text, string value)
        {
            int index = text.IndexOf(value);
            if (index < 0)
            {
                return text;
            }

            return text.Substring(index + value.Length);
        }

        public static string RemoveLastOf(this string text, string value)
        {
            int index = text.LastIndexOf(value);
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

        public static bool StartsWith(this string text, string[] prefixes)
        {
            foreach (string prefix in prefixes)
            {
                if (text.StartsWith(prefix))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsWith(this string text, string[] postfixes)
        {
            foreach (string postfix in postfixes)
            {
                if (text.EndsWith(postfix))
                {
                    return true;
                }
            }

            return false;
        }

        public static int SearchOfBm(this string text, string pattern, bool isIgnoreCase = true)
        {
            return StringSearcher.SearchOfBm(text, pattern, isIgnoreCase);
        }

        #endregion

        #region StringBuilder

        public static void RemoveFirstCount(this StringBuilder builder, int count = 1)
        {
            if (builder.Length < count)
            {
                return;
            }

            builder.Remove(0, count);
        }

        public static void RemoveLastCount(this StringBuilder builder, int count = 1)
        {
            if (builder.Length < count)
            {
                return;
            }

            builder.Remove(builder.Length - count, count);
        }

        public static void AppendPadLeft(this StringBuilder builder, string text, int width)
        {
            builder.Append(' ', width);
            builder.Append(text);
        }

        #endregion
    }
}