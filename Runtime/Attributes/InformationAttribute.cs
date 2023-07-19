using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InformationAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public readonly string Message;
        public readonly UnityEditor.MessageType Type;
        public readonly bool MessageAfterProperty;

        public InformationAttribute(string message, InformationType type = InformationType.Info, bool messageAfterProperty = false)
        {
            Message = message;
            Type = (UnityEditor.MessageType) type;
            MessageAfterProperty = messageAfterProperty;
        }
#else
        public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
        }
#endif
    }

    public enum InformationType
    {
        None,
        Info,
        Waring,
        Error
    }
}