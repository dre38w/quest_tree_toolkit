#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Service.Framework.Goals;
using UnityEditor;
using UnityEngine;

namespace Support.Editor
{
    [CustomPropertyDrawer(typeof(GoalRequirement), true)]
    public class GoalRequirementDrawer : PropertyDrawer
    {
        private static List<Type> types;

        private static List<Type> GetAllTypes()
        {
            if (types != null)
            {
                return types;
            }
            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(GoalRequirement).IsAssignableFrom(t)).ToList();
            return types;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                SerializedProperty iterator = property.Copy();
                int depth = iterator.depth;

                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (iterator.depth <= depth)
                        {
                            break;
                        }
                        height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
                    }
                    while (iterator.NextVisible(false));
                }
            }
            return height + 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            string currentTypeName = property.managedReferenceValue != null ? property.managedReferenceValue.GetType().Name : "None";

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();

                foreach (Type type in GetAllTypes())
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                //menu.AddSeparator("");
                //menu.AddItem(new GUIContent("Clear"), false, () =>
                //{
                //    property.managedReferenceValue = null;
                //    property.serializedObject.ApplyModifiedProperties();
                //});
                menu.ShowAsContext();
            }
            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;

                SerializedProperty iterator = property.Copy();
                int depth = iterator.depth;

                float rectY = position.y + EditorGUIUtility.singleLineHeight + 4;

                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (iterator.depth <= depth)
                        {
                            break;
                        }
                        float height = EditorGUI.GetPropertyHeight(iterator, true);
                        Rect fieldRect = new Rect(position.x, rectY, position.width, height);

                        EditorGUI.PropertyField(fieldRect, iterator, true);

                        rectY += height + 2;
                    }
                    while (iterator.NextVisible(false));
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif