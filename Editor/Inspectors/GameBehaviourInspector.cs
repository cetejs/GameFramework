using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameBehaviour), true, isFallback = true)]
    internal class GameBehaviourInspector : Editor
    {
        private List<SerializedProperty> propertyList = new List<SerializedProperty>();
        private Dictionary<string, GroupData> groupDataDict = new Dictionary<string, GroupData>();
        private bool showDefaultInspector;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        protected virtual void OnEnable()
        {
            CollectGroupAttributes();
        }

        protected virtual void OnDisable()
        {
            if (target == null)
            {
                return;
            }

            foreach (GroupData groupData in groupDataDict.Values)
            {
                if (groupData.Properties.Count > 0)
                {
                    EditorPrefs.SetBool($"{groupData.GroupAttribute.GroupName}{groupData.Properties[0].name}{target.GetInstanceID()}", groupData.GroupIsOpen);
                }

                groupData.Clear();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawBase();
            DrawScriptBox();
            DrawContainer();
            DrawContents();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void CollectGroupAttributes()
        {
            groupDataDict.Clear();
            InspectorGroupAttribute previousGroupAttribute = null;
            List<FieldInfo> fieldInfos = AssemblyUtils.GetFieldInfos(target.GetType());

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsDefined(typeof(HideInInspector), true))
                {
                    continue;
                }

                InspectorGroupAttribute groupAttribute = fieldInfo.GetCustomAttribute<InspectorGroupAttribute>();
                GroupData groupData;
                if (groupAttribute == null)
                {
                    if (previousGroupAttribute != null && previousGroupAttribute.GroupAllFieldsUntilNextGroupAttribute)
                    {
                        showDefaultInspector = false;
                        if (!groupDataDict.TryGetValue(previousGroupAttribute.GroupName, out groupData))
                        {
                            groupDataDict.Add(previousGroupAttribute.GroupName, new GroupData
                            {
                                GroupAttribute = previousGroupAttribute,
                                FieldNames = new HashSet<string> {fieldInfo.Name},
                                GroupColor = ColorUtils.GetColorAt(previousGroupAttribute.GroupColorIndex)
                            });
                        }
                        else
                        {
                            groupData.GroupColor = ColorUtils.GetColorAt(previousGroupAttribute.GroupColorIndex);
                            groupData.FieldNames.Add(fieldInfo.Name);
                        }
                    }
                }

                if (groupAttribute != null)
                {
                    previousGroupAttribute = groupAttribute;

                    if (!groupDataDict.TryGetValue(groupAttribute.GroupName, out groupData))
                    {
                        bool groupIsOpen = EditorPrefs.GetBool($"{groupAttribute.GroupName}{fieldInfo.Name}{target.GetInstanceID()}", false);
                        groupDataDict.Add(groupAttribute.GroupName, new GroupData
                            {
                                GroupAttribute = groupAttribute,
                                GroupColor = ColorUtils.GetColorAt(previousGroupAttribute.GroupColorIndex),
                                FieldNames = new HashSet<string> {fieldInfo.Name}, GroupIsOpen = groupIsOpen
                            }
                        );
                    }
                    else
                    {
                        groupData.FieldNames.Add(fieldInfo.Name);
                        groupData.GroupColor = ColorUtils.GetColorAt(previousGroupAttribute.GroupColorIndex);
                    }
                }
            }

            SerializedProperty iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    FillProperties(iterator);
                }
                while (iterator.NextVisible(false));
            }
        }

        protected virtual void DrawBase()
        {
            if (showDefaultInspector)
            {
                DrawDefaultInspector();
            }
        }

        protected virtual void DrawScriptBox()
        {
            if (propertyList.Count == 0)
            {
                return;
            }

            using (new EditorGUI.DisabledScope("m_Script" == propertyList[0].propertyPath))
            {
                EditorGUILayout.PropertyField(propertyList[0], true);
            }
        }

        protected virtual void DrawContainer()
        {
            if (propertyList.Count == 0)
            {
                return;
            }

            foreach (GroupData groupData in groupDataDict.Values)
            {
                EditorGUILayout.BeginVertical(InspectorStyle.BehaviourContainerStyle);
                DrawGroup(groupData);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = 0;
            }
        }

        protected virtual void DrawContents()
        {
            if (propertyList.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();
            for (int i = 1; i < propertyList.Count; i++)
            {
                EditorGUILayout.PropertyField(propertyList[i], true);
            }
        }

        protected virtual void DrawGroup(GroupData groupData)
        {
            Rect verticalGroup = EditorGUILayout.BeginVertical();

            Rect leftBorderRect = new Rect(verticalGroup.xMin + 5, verticalGroup.yMin - 10, 3f, verticalGroup.height + 20);
            leftBorderRect.xMin = 15f;
            leftBorderRect.xMax = 18f;
            EditorGUI.DrawRect(leftBorderRect, groupData.GroupColor);

            groupData.GroupIsOpen = EditorGUILayout.Foldout(groupData.GroupIsOpen, groupData.GroupAttribute.GroupName, true, InspectorStyle.BehaviourGroupStyle);

            if (groupData.GroupIsOpen)
            {
                EditorGUI.indentLevel = 0;

                for (int i = 0; i < groupData.Properties.Count; i++)
                {
                    EditorGUILayout.BeginVertical(InspectorStyle.BehaviourBoxChildStyle);
                    DrawChild(groupData, i);
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawChild(GroupData groupData, int index)
        {
            EditorGUILayout.PropertyField(groupData.Properties[index], new GUIContent(groupData.Properties[index].displayName, tooltip: groupData.Properties[index].tooltip), true);
        }

        private void FillProperties(SerializedProperty serializedProperty)
        {
            bool shouldClose = false;

            foreach (GroupData groupData in groupDataDict.Values)
            {
                if (groupData.FieldNames.Contains(serializedProperty.name))
                {
                    SerializedProperty property = serializedProperty.Copy();
                    shouldClose = true;
                    groupData.Properties.Add(property);
                    break;
                }
            }

            if (!shouldClose)
            {
                SerializedProperty property = serializedProperty.Copy();
                propertyList.Add(property);
            }
        }

        protected class GroupData
        {
            public bool GroupIsOpen;
            public InspectorGroupAttribute GroupAttribute;
            public List<SerializedProperty> Properties = new List<SerializedProperty>();
            public HashSet<string> FieldNames = new HashSet<string>();
            public Color GroupColor;

            public void Clear()
            {
                GroupAttribute = null;
                Properties.Clear();
                FieldNames.Clear();
            }
        }
    }
}