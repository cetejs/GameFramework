using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorGroupAttribute : PropertyAttribute
    {
        public readonly string GroupName;
        public readonly bool GroupAllFieldsUntilNextGroupAttribute;
        public readonly int GroupColorIndex;
        
        public InspectorGroupAttribute(string groupName, int groupColorIndex)
        {
            GroupName = groupName;
            GroupAllFieldsUntilNextGroupAttribute = true;
            GroupColorIndex = groupColorIndex;
        }

        public InspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = true, int groupColorIndex = 0)
        {
            GroupName = groupName;
            GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
            GroupColorIndex = groupColorIndex;
        }
    }
}