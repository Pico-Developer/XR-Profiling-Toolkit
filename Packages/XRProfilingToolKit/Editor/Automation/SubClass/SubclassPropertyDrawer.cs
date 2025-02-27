/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DeveloperTech.XRProfilingToolkit.Utilities
{
    /// <summary>
    /// Custom property drawer for the serialized field with <see cref="SubclassAttribute"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(SubclassAttribute))]
    public class SubclassPropertyDrawer : PropertyDrawer
    {
        private readonly Dictionary<System.Type, SubclassSelector> _selectorMap = new();

        private const float Spacing = 2.0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float lineSize = EditorGUIUtility.singleLineHeight;
            float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginProperty(position, label, property);
            {
                position.x -= Spacing;
                position.width += Spacing * 2.0f;
                position.y -= Spacing;
                position.height += Spacing * 2.0f;

                EditorGUI.DrawRect(position, new Color(0.28f, 0.28f, 0.28f));
                position.x += Spacing;
                position.y += Spacing;

                position.width -= Spacing * 2.0f;
                position.height -= Spacing * 2.0f;

                SubclassAttribute propertyAtt = attribute as SubclassAttribute;

                SubclassSelector selector = null;

                System.Type selectType = null;

                selectType = fieldInfo.FieldType;

                if (selectType.IsArray)
                    selectType = selectType.GetElementType();

                if (propertyAtt.IsList)
                {
                    System.Type[] genericTypes = selectType.GetGenericArguments();

                    if (genericTypes.Length <= 0)
                        return;

                    selectType = genericTypes[0];
                }

                if (!_selectorMap.TryGetValue(selectType, out selector))
                {
                    selector = new SubclassSelector(selectType,
                        property.managedReferenceValue?.GetType(),
                        propertyAtt.IncludeSelf);
                    _selectorMap.Add(selectType, selector);
                }
                else
                {
                    selector.RefreshSelection(property.managedReferenceValue?.GetType());
                }

                position.height = lineSize;

                if (selector.Draw(position))
                {
                    property.managedReferenceValue = selector.CreateSelected();
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    position.y += lineSize;

                    EditorGUI.PropertyField(position, property, label);

                    bool indented = false;

                    if (EditorGUI.indentLevel <= 0)
                    {
                        EditorGUI.indentLevel++;
                        indented = true;
                    }

                    position.y += lineSize;

                    SerializedProperty lastProp = property.GetEndProperty();

                    if (property.hasVisibleChildren && property.isExpanded)
                    {
                        SerializedProperty curProp = property;

                        if (curProp.NextVisible(true))
                        {
                            while (!SerializedProperty.EqualContents(curProp, lastProp))
                            {
                                position.height = EditorGUI.GetPropertyHeight(curProp);
                                EditorGUI.PropertyField(position, curProp);

                                position.y += fieldSpacing;

                                if (curProp.hasChildren && !curProp.isArray && curProp.isExpanded)
                                {
                                    position.y += lineSize;
                                }
                                else
                                {
                                    float propHeight = EditorGUI.GetPropertyHeight(curProp);
                                    position.y += propHeight;
                                }

                                if (!curProp.NextVisible(!curProp.isArray && curProp.isExpanded))
                                    break;
                            }
                        }
                    }

                    if (indented)
                        EditorGUI.indentLevel--;
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineSize = EditorGUIUtility.singleLineHeight;
            float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;

            float totalHeight = lineSize + (Spacing * 2.0f);

            SerializedProperty lastProp = property.GetEndProperty();

            if (property.hasVisibleChildren && property.isExpanded)
            {
                SerializedProperty curProp = property;

                if (curProp.NextVisible(true))
                {
                    while (!SerializedProperty.EqualContents(curProp, lastProp))
                    {
                        totalHeight += fieldSpacing;

                        if (curProp.hasChildren && !curProp.isArray && curProp.isExpanded)
                        {
                            totalHeight += lineSize;
                        }
                        else
                        {
                            float propHeight = EditorGUI.GetPropertyHeight(curProp);
                            totalHeight += propHeight;
                        }

                        if (!curProp.NextVisible(!curProp.isArray && curProp.isExpanded))
                            break;
                    }
                }
            }

            return base.GetPropertyHeight(property, label) + totalHeight;
        }
    }
}