/*
 * Description: Custom editor script to allow setting references to abstract classes
 */
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Service.Core;
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
            //create the list of abstract goal requirement classes to add to the drop down
            //exclude the Default Requirement as that is used as a safeguard against lost references and should not be manually added
            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(GoalRequirement).IsAssignableFrom(t) &&
                t != typeof(DefaultRequirement)).ToList();
            return types;
        }

        /// <summary>
        /// In the event a serialized reference is lost, automatically fix it
        /// </summary>
        /// <param name="property"></param>
        private void ValidateReferences(SerializedProperty property)
        {
            UnityEngine.Object target = property.serializedObject.targetObject;

            //make sure we only work with the Goal class
            if (!(target is Goal goal))
            {
                return;
            }

            bool refChanged = false;
            List<GoalRequirement> newReqList = new List<GoalRequirement>();

            //rebuild the list
            foreach (GoalRequirement req in goal.Requirements)
            {
                //use the requirement that is still valid
                if (req != null)
                {
                    newReqList.Add(req);
                }
                //if a reference was missing, replace with the default requirement
                else
                {
                    newReqList.Add(new DefaultRequirement());
                    refChanged = true;
                }
            }
            //now apply the rebuilt requirements list
            if (refChanged)
            {
                goal.ResetRequirements(newReqList);
                EditorUtility.SetDirty(goal);
            }
            //clear Unity's internal flag that gets set when managed serialized references break
            if (SerializationUtility.HasManagedReferencesWithMissingTypes(target))
            {
                SerializationUtility.ClearAllManagedReferencesWithMissingTypes(target);
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Set the spacing for our custom Inspector
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                SerializedProperty iterator = property.Copy();
                int depth = iterator.depth;

                //iterate to create proper spacing
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
            //check to see if references need to be rebuilt
            ValidateReferences(property);

            EditorGUI.BeginProperty(position, label, property);

            float currentY = position.y;
            //the display name of the requirement
            string currentTypeName = property.managedReferenceValue != null ? property.managedReferenceValue.GetType().Name : "None";

            Rect dropdownRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();

                //set a reference to the requirement instances and add them to the dropdown menu
                foreach (Type type in GetAllTypes())
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            float rectY = currentY + EditorGUIUtility.singleLineHeight + 4;

            //display the data
            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;

                SerializedProperty iterator = property.Copy();
                int depth = iterator.depth;

                //iterate over to create the display data
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
                        //draw the field
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