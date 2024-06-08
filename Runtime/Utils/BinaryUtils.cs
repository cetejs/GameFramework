using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameFramework
{
    public static class BinaryUtils
    {
        private static byte[] buffer;
        private const int BufferCapacity = 1024;

        private static void EnsureInstance()
        {
            if (buffer == null)
            {
                buffer = new byte[BufferCapacity];
            }
        }

        public static byte[] ToBinary(object obj)
        {
            EnsureInstance();

            try
            {
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    byte[] bytes = new byte[stream.Position];
                    Array.Copy(buffer, bytes, bytes.Length);
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
            }

            return null;
        }

        public static object ToObject(byte[] bytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
            }

            return default;
        }

        public static bool IsComplexObject<T>(T value)
        {
            return !(typeof(T).IsPrimitive || value is string);
        }

        public static bool IsComplexObject(Type type)
        {
            return !(type.IsPrimitive || type == typeof(string));
        }

        public static byte[] ConvertToBinary<T>(T obj)
        {
            if (obj == null)
            {
                GameLogger.LogError("Object to bytes is fail, because obj is invalid");
                return null;
            }

            if (IsComplexObject(obj))
            {
                return ToBinary(obj);
            }

            EnsureInstance();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    if (obj is bool @bool)
                    {
                        writer.Write(@bool);
                    }
                    else if (obj is byte @byte)
                    {
                        writer.Write(@byte);
                    }
                    else if (obj is sbyte @sbyte)
                    {
                        writer.Write(@sbyte);
                    }
                    else if (obj is char @char)
                    {
                        writer.Write(@char);
                    }
                    else if (obj is double @double)
                    {
                        writer.Write(@double);
                    }
                    else if (obj is decimal @decimal)
                    {
                        writer.Write(@decimal);
                    }
                    else if (obj is short @short)
                    {
                        writer.Write(@short);
                    }
                    else if (obj is ushort @ushort)
                    {
                        writer.Write(@ushort);
                    }
                    else if (obj is int @int)
                    {
                        writer.Write(@int);
                    }
                    else if (obj is uint @uint)
                    {
                        writer.Write(@uint);
                    }
                    else if (obj is long @long)
                    {
                        writer.Write(@long);
                    }
                    else if (obj is ulong @ulong)
                    {
                        writer.Write(@ulong);
                    }
                    else if (obj is float @float)
                    {
                        writer.Write(@float);
                    }
                    else
                    {
                        writer.Write(obj.ToString());
                    }

                    byte[] bytes = new byte[stream.Position];
                    Array.Copy(buffer, bytes, bytes.Length);
                    return bytes;
                }
            }
        }

        public static T ConvertToObject<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                GameLogger.LogError("Bytes to object is fail, because bytes is invalid");
                return default;
            }

            return (T) ConvertToObject(bytes, typeof(T));
        }

        public static object ConvertToObject(byte[] bytes, Type type)
        {
            if (bytes == null)
            {
                GameLogger.LogError("Bytes to object is fail, because bytes is invalid");
                return null;
            }

            if (IsComplexObject(type))
            {
                return ToObject(bytes);
            }

            using (MemoryStream tempStream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(tempStream))
                {
                    object value;
                    if (type == typeof(bool))
                    {
                        value = reader.ReadBoolean();
                    }
                    else if (type == typeof(byte))
                    {
                        value = reader.ReadByte();
                    }
                    else if (type == typeof(sbyte))
                    {
                        value = reader.ReadSByte();
                    }
                    else if (type == typeof(char))
                    {
                        value = reader.ReadChar();
                    }
                    else if (type == typeof(double))
                    {
                        value = reader.ReadDouble();
                    }
                    else if (type == typeof(decimal))
                    {
                        value = reader.ReadDecimal();
                    }
                    else if (type == typeof(short))
                    {
                        value = reader.ReadInt16();
                    }
                    else if (type == typeof(ushort))
                    {
                        value = reader.ReadUInt16();
                    }
                    else if (type == typeof(int))
                    {
                        value = reader.ReadInt32();
                    }
                    else if (type == typeof(uint))
                    {
                        value = reader.ReadUInt32();
                    }
                    else if (type == typeof(long))
                    {
                        value = reader.ReadInt64();
                    }
                    else if (type == typeof(ulong))
                    {
                        value = reader.ReadUInt64();
                    }
                    else if (type == typeof(float))
                    {
                        value = reader.ReadSingle();
                    }
                    else
                    {
                        value = reader.ReadString();
                    }

                    return value;
                }
            }
        }
    }
}