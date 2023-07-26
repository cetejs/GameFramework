using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumPopUpAttribute : PropertyAttribute
    {
        public readonly string[] DisplayedOptions;

        public EnumPopUpAttribute(params string[] displayedOptions)
        {
            DisplayedOptions = displayedOptions;
        }
    }
}