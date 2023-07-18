using System;
using LitJson;

namespace GameFramework
{
    public static class JsonUtils
    {
        public static string ToJson(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public static T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }

        public static bool IsComplexObject<T>(T value)
        {
            return !(typeof(T).IsPrimitive || value is string);
        }

        public static bool IsComplexObject(Type type)
        {
            return !(type.IsPrimitive || type == typeof(string));
        }

        public static string ConvertToJson<T>(T obj)
        {
            if (obj == null)
            {
                GameLogger.LogError("Object is invalid");
                return null;
            }

            if (IsComplexObject(obj))
            {
                return ToJson(obj);
            }

            if (obj is bool)
            {
                return Convert.ToByte(obj).ToString();
            }

            return obj.ToString();
        }

        public static T ConvertToObject<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                GameLogger.LogError("Text is invalid");
                return default;
            }

            return (T) ConvertToObject(text, typeof(T));
        }

        public static object ConvertToObject(string text, Type type)
        {
            if (string.IsNullOrEmpty(text))
            {
                GameLogger.LogError("Text is invalid");
                return null;
            }

            if (IsComplexObject(type))
            {
                return JsonMapper.ToObject(text, type);
            }

            object value;
            if (type == typeof(bool))
            {
                value = byte.Parse(text) > 0;
            }
            else if (type == typeof(char))
            {
                value = char.Parse(text);
            }
            else if (type == typeof(byte))
            {
                value = byte.Parse(text);
            }
            else if (type == typeof(sbyte))
            {
                value = sbyte.Parse(text);
            }
            else if (type == typeof(short))
            {
                value = short.Parse(text);
            }
            else if (type == typeof(ushort))
            {
                value = ushort.Parse(text);
            }
            else if (type == typeof(int))
            {
                value = int.Parse(text);
            }
            else if (type == typeof(uint))
            {
                value = uint.Parse(text);
            }
            else if (type == typeof(long))
            {
                value = long.Parse(text);
            }
            else if (type == typeof(ulong))
            {
                value = ulong.Parse(text);
            }
            else if (type == typeof(float))
            {
                value = float.Parse(text);
            }
            else if (type == typeof(double))
            {
                value = double.Parse(text);
            }
            else
            {
                value = text;
            }

            return value;
        }
    }
}