using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public bool IsReadOnly
        {
            get { return EnableInEditor || Application.isPlaying; }
        }

        public readonly bool EnableInEditor;
        
        public ReadOnlyAttribute()
        {
            EnableInEditor = true;
        }

        public ReadOnlyAttribute(bool enableInEditor)
        {
            EnableInEditor = enableInEditor;
        }
    }
}