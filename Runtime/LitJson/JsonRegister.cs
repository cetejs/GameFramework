using System;
using UnityEngine;

namespace LitJson
{
    internal static class JsonRegister
    {
        private static bool isRegistered;

        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            if (isRegistered)
            {
                return;
            }

            isRegistered = true;
            RegisterType();
            RegisterVector2();
            RegisterVector3();
            RegisterVector4();
            RegisterQuaternion();
            RegisterColor();
            RegisterColor32();
        }

        private static void RegisterType()
        {
            void Exporter(Type obj, JsonWriter writer)
            {
                writer.Write(obj.FullName);
            }

            JsonMapper.RegisterExporter((ExporterFunc<Type>) Exporter);

            Type Importer(string obj)
            {
                return Type.GetType(obj);
            }

            JsonMapper.RegisterImporter((ImporterFunc<string, Type>) Importer);
        }

        private static void RegisterVector2()
        {
            void Exporter(Vector2 obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("x", obj.x);
                writer.WriteProperty("y", obj.y);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Vector2>) Exporter);
        }

        private static void RegisterVector3()
        {
            void Exporter(Vector3 obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("x", obj.x);
                writer.WriteProperty("y", obj.y);
                writer.WriteProperty("z", obj.z);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Vector3>) Exporter);
        }

        private static void RegisterVector4()
        {
            void Exporter(Vector4 obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("x", obj.x);
                writer.WriteProperty("y", obj.y);
                writer.WriteProperty("z", obj.z);
                writer.WriteProperty("w", obj.w);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Vector4>) Exporter);
        }

        private static void RegisterQuaternion()
        {
            void Exporter(Quaternion obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("x", obj.x);
                writer.WriteProperty("y", obj.y);
                writer.WriteProperty("z", obj.z);
                writer.WriteProperty("w", obj.w);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Quaternion>) Exporter);
        }

        private static void RegisterColor()
        {
            void Exporter(Color obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("r", obj.r);
                writer.WriteProperty("g", obj.g);
                writer.WriteProperty("b", obj.b);
                writer.WriteProperty("a", obj.a);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Color>) Exporter);
        }

        private static void RegisterColor32()
        {
            void Exporter(Color32 obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteProperty("r", obj.r);
                writer.WriteProperty("g", obj.g);
                writer.WriteProperty("b", obj.b);
                writer.WriteProperty("a", obj.a);
                writer.WriteObjectEnd();
            }

            JsonMapper.RegisterExporter((ExporterFunc<Color32>) Exporter);
        }

        private static void WriteProperty(this JsonWriter writer, string name, int value)
        {
            writer.WritePropertyName(name);
            writer.Write(value);
        }

        private static void WriteProperty(this JsonWriter writer, string name, float value)
        {
            writer.WritePropertyName(name);
            writer.Write(value);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        public JsonIgnoreAttribute()
        {
        }
    }
}