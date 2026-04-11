#if UNITY_EDITOR
using PlasticGui.WorkspaceWindow;
using Service.Framework.Goals;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Support.Editor
{
    public class QuestTreeEditorWindow : EditorWindow
    {
        private const float MIN_LEFT_COLUMN_WIDTH = 180f;
        private const float MIN_RIGHT_COLUMN_WIDTH = 260f;
        private const float MIN_CENTER_COLUMN_WIDTH = 240f;
        private const float SPLITTER_WIDTH = 4f;
        private const int MAX_BREADCRUMBS_VISIBLE = 4;

        private float leftWidth = 220f;
        private float rightWidth = 320f;

        private bool isDraggingLeftSplitter;
        private bool isDraggingRightSplitter;
        private bool isResizingLeft;
        private bool isResizingRight;

        private bool goalListNeedsRefresh = true; //confirm the point

        private Vector2 leftColumnScroll;
        private Vector2 centerColumnScroll;
        private Vector2 rightColumnScroll;

        private UnityObject currentNode;
        private UnityObject selectedNode;

        private UnityEditor.Editor cachedEditor;
        private ReorderableList childrenList; //probably change the name
        private SerializedObject childrenSerializedObject; //change the name

        private Goal selectedGoal;

        private Goal goalToDelete;
        private ObjectiveAction actionToDelete;
        private UnityObject nodeParent;
        private int nodeIndexToDelete = -1;

        private ReorderableList goalList;
        private List<Goal> cachedGoals = new List<Goal>();

        private readonly Stack<UnityObject> navigationStack = new Stack<UnityObject>();

        [MenuItem("Tools/Quest Tree Editor")]
        public static void ShowWindow()
        {
            GetWindow<QuestTreeEditorWindow>("Quest Tree Editor");
        }

        private void OnGUI()
        {
            //clamp the window to be no smaller than the sum of the columns' widths
            //this will prevent the columns from overlapping each other if resized too small
            minSize = new Vector2(MIN_LEFT_COLUMN_WIDTH + MIN_CENTER_COLUMN_WIDTH + MIN_RIGHT_COLUMN_WIDTH + (SPLITTER_WIDTH * 2f), 300f);

            HandleSplitters();
            ClampPanelWidths();

            float windowWidth = position.width;
            float windowHeight = position.height;

            Rect leftRect = new Rect(0, 0, leftWidth, windowHeight);
            Rect leftSplitterRect = new Rect(leftRect.xMax, 0, SPLITTER_WIDTH, windowHeight);

            Rect rightRect = new Rect(windowWidth - rightWidth, 0, rightWidth, windowHeight);
            Rect rightSplitterRect = new Rect(rightRect.xMin - SPLITTER_WIDTH, 0, SPLITTER_WIDTH, windowHeight);

            Rect centerRect = new Rect(leftSplitterRect.xMax, 0f, rightSplitterRect.xMin - leftSplitterRect.xMax, windowHeight);

            //draw left column
            GUILayout.BeginArea(leftRect);
            DrawLeftPanel();
            GUILayout.EndArea();

            //draw center column
            GUILayout.BeginArea(centerRect);
            DrawCenterPanel();
            GUILayout.EndArea();

            //draw right column
            GUILayout.BeginArea(rightRect);
            DrawRightPanel();
            GUILayout.EndArea();

            DrawSplitter(leftSplitterRect);
            DrawSplitter(rightSplitterRect);

            HandleDeferredDeletes();
        }

        #region Handle Window
        private void HandleSplitters()
        {
            Event currentEvent = Event.current;

            Rect leftSplitterRect = new Rect(leftWidth, 0f, SPLITTER_WIDTH, position.height);
            Rect rightSplitterRect = new Rect(position.width - rightWidth - SPLITTER_WIDTH, 0f, SPLITTER_WIDTH, position.height);

            EditorGUIUtility.AddCursorRect(leftSplitterRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(rightSplitterRect, MouseCursor.ResizeHorizontal);

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (leftSplitterRect.Contains(currentEvent.mousePosition))
                    {
                        isDraggingLeftSplitter = true;
                        currentEvent.Use();
                    }
                    else if (rightSplitterRect.Contains(currentEvent.mousePosition))
                    {
                        isDraggingRightSplitter = true;
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (isDraggingLeftSplitter)
                    {
                        leftWidth = currentEvent.mousePosition.x;
                        ClampPanelWidths();
                        Repaint();
                        currentEvent.Use();
                    }
                    else if (isDraggingRightSplitter)
                    {
                        rightWidth = position.width - currentEvent.mousePosition.x - SPLITTER_WIDTH;
                        ClampPanelWidths();
                        Repaint();
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (isDraggingLeftSplitter || isDraggingRightSplitter)
                    {
                        isDraggingLeftSplitter = false;
                        isDraggingRightSplitter = false;
                        currentEvent.Use();
                    }
                    break;
            }
        }

        private void DrawSplitter(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f, 1));
        }

        private void ClampPanelWidths()
        {
            float totalWidth = position.width;
            float availableSpaceForPanels = totalWidth - (SPLITTER_WIDTH * 2f);

            float maxLeft = availableSpaceForPanels - MIN_CENTER_COLUMN_WIDTH - MIN_RIGHT_COLUMN_WIDTH;
            float maxRight = availableSpaceForPanels - MIN_CENTER_COLUMN_WIDTH - MIN_LEFT_COLUMN_WIDTH;

            maxLeft = Mathf.Max(MIN_LEFT_COLUMN_WIDTH, maxLeft);
            maxRight = Mathf.Max(MIN_RIGHT_COLUMN_WIDTH, maxRight);

            leftWidth = Mathf.Clamp(leftWidth, MIN_LEFT_COLUMN_WIDTH, maxLeft);
            rightWidth = Mathf.Clamp(rightWidth, MIN_RIGHT_COLUMN_WIDTH, maxRight);

/* TODO */  float centerWidth = availableSpaceForPanels - leftWidth - rightWidth; //TODO: comment out to see what happens

            if (centerWidth < MIN_CENTER_COLUMN_WIDTH)
            {
/* TODO */      float widthDeficit = MIN_CENTER_COLUMN_WIDTH - centerWidth; //what this?

                //shrink the larger side first for UX
                if (rightWidth > leftWidth)
                {
                    float newRight = Mathf.Max(MIN_RIGHT_COLUMN_WIDTH, rightWidth - widthDeficit);
                    widthDeficit -= rightWidth - newRight;
                    rightWidth = newRight;

                    if (widthDeficit > 0f)
                    {
                        leftWidth = Mathf.Max(MIN_LEFT_COLUMN_WIDTH, leftWidth - widthDeficit);
                    }
                }
                else
                {
                    float newLeft = Mathf.Max(MIN_LEFT_COLUMN_WIDTH, leftWidth - widthDeficit);
                    widthDeficit -= leftWidth - newLeft;
                    leftWidth = newLeft;

                    if (widthDeficit > 0f)
                    {
                        rightWidth = Mathf.Max(MIN_RIGHT_COLUMN_WIDTH, rightWidth - widthDeficit);
                    }
                }
            }
        }

        private void HandleResize(Rect splitter, ref float newSize, bool isLeft)
        {
            EditorGUIUtility.AddCursorRect(splitter, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && splitter.Contains(Event.current.mousePosition))
            {
                if (isLeft)
                {
                    isResizingLeft = true;
                }
                else
                {
                    isResizingRight = true;
                }
            }
            if (Event.current.type == EventType.MouseDrag)
            {
                if (isLeft && isResizingLeft)
                {
                    newSize = Mathf.Clamp(Event.current.mousePosition.x, MIN_LEFT_COLUMN_WIDTH, position.width - rightWidth - MIN_CENTER_COLUMN_WIDTH);
                    Repaint();
                }
            }
            if (Event.current.type == EventType.MouseUp)
            {
                isResizingLeft = false;
                isResizingRight = false;
            }
        }
        #endregion

        #region Configure and manage left column and the list of Goals
        /// <summary>
        /// Left panel contains a list of all the Goals in the scene
        /// </summary>
        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Goals", EditorStyles.boldLabel);

            ValidateGoalList();

            leftColumnScroll = EditorGUILayout.BeginScrollView(leftColumnScroll);

            if (goalList != null)
            {
                goalList.DoLayoutList();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Create New Goal"))
            {
                ShowCreateGoalMenu();
                //CreateNewGoal();
                //goalListNeedsRefresh = true;
            }
            EditorGUILayout.EndVertical();
        }

        private void ValidateGoalList()
        {
            if (!goalListNeedsRefresh && goalList != null)
            {
                return;
            }
            cachedGoals.Clear();
            cachedGoals.AddRange(FindObjectsByType<Goal>(FindObjectsSortMode.None));

            //sort by sibling index so we can match the hierarchy panel's order
            cachedGoals.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            //make the goal list a reorderable for better organization
            goalList = new ReorderableList(cachedGoals, typeof(Goal), true, true, false, false);

            goalList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Goals");
            };

            goalList.elementHeight = EditorGUIUtility.singleLineHeight + 6f;

            goalList.drawElementCallback = (rect, index, active, focused) =>
            {
                Goal goal = cachedGoals[index];
                if (goal == null)
                {
                    return;
                }
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;

                float width = rect.width;
                //the goal's name field
                Rect nameRect = new Rect(rect.x, rect.y, width - 120f, rect.height);
                //the select goal button
                Rect selectRect = new Rect(rect.x + width - 115f, rect.y, 55f, rect.height);
                //the delete goal button
                Rect deleteRect = new Rect(rect.x + width - 55f, rect.y, 50f, rect.height);

                string newName = EditorGUI.TextField(nameRect, goal.name);

                if (newName != goal.name)
                {
                    RenameObject(goal, newName);
                }

                if (GUI.Button(selectRect, "Select"))
                {
                    SelectGoal(goal);
                }
                if (GUI.Button(deleteRect, "Delete"))
                {
                    goalToDelete = goal;
                }
            };

            goalList.onReorderCallback = list =>
            {
                for (int i = 0; i < cachedGoals.Count; i++)
                {
                    if (cachedGoals[i] != null)
                    {
                        Undo.RecordObject(cachedGoals[i].transform, "Reorder Goals");
                        cachedGoals[i].transform.SetSiblingIndex(i);
                    }
                }
                goalListNeedsRefresh = true;
            };
            goalListNeedsRefresh = false;
        }

        private void RefreshGoalList()
        {
            cachedGoals.Clear();
            cachedGoals.AddRange(FindObjectsByType<Goal>(FindObjectsSortMode.None));
            goalList = new ReorderableList(cachedGoals, typeof(Goal), true, true, false, false);

            goalList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Goals");
            };
            goalList.drawElementCallback = (rect, index, active, focused) =>
            {
                Goal goal = cachedGoals[index];

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                float rectWidth = rect.width;

                Rect nameRect = new Rect(rect.x, rect.y, rectWidth - 120f, rect.height);
                Rect selectRect = new Rect(rect.x + rectWidth - 115f, rect.y, 55f, rect.height);
                Rect deleteRect = new Rect(rect.x + rectWidth - 55f, rect.y, 50f, rect.height);

                string newName = EditorGUI.TextField(nameRect, goal.name);

                if (newName != goal.name)
                {
                    RenameObject(goal, newName);
                }

                if (GUI.Button(selectRect, "Select"))
                {
                    SelectGoal(goal);
                }
                if (GUI.Button(deleteRect, "Delete"))
                {
                    goalToDelete = goal;
                }
            };

            goalList.onReorderCallback = list =>
            {
                for (int i = 0; i < cachedGoals.Count; i++)
                {
                    if (cachedGoals[i] != null)
                    {
                        cachedGoals[i].transform.SetSiblingIndex(i);
                    }
                }
            };
        }

        private void GenerateGoalCallbacks()
        {

        }
        #endregion



        #region Handle drawing the center column and manage the Actions in it
        private void DrawCenterPanel()
        {
            EditorGUILayout.BeginVertical();

            if (selectedGoal == null)
            {
                EditorGUILayout.HelpBox("Select a Goal", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }
            DrawHeader();

            centerColumnScroll = EditorGUILayout.BeginScrollView(centerColumnScroll);
            DrawChildrenList();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawBreadcrumbs();

            EditorGUILayout.BeginHorizontal();

            string currentName = currentNode != null ? currentNode.name : string.Empty;
            string editedName = EditorGUILayout.TextField(currentName);

            if (currentNode != null && editedName != currentName)
            {
                RenameObject(currentNode, editedName);
            }
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(navigationStack.Count == 0))
            {
                if (GUILayout.Button("Back", GUILayout.Width(70)))
                {
                    NavigateBack();
                }
            }
            if (GUILayout.Button("Add Action", GUILayout.Width(90)))
            {
                ShowAddMenu();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawBreadcrumbs()
        {
            List<UnityObject> path = GetNavigationPath();

            EditorGUILayout.BeginHorizontal();

            int startIndex = 0;

            if (path.Count > MAX_BREADCRUMBS_VISIBLE)
            {
                startIndex = path.Count - MAX_BREADCRUMBS_VISIBLE;

                //truncate the breadcrumbs when in too many layers
                GUILayout.Label("...", GUILayout.Width(25));
                GUILayout.Label(">", GUILayout.Width(10));
            }

            for (int i = startIndex; i < path.Count; i++)
            {
                UnityObject node = path[i];
                if (node == null)
                {
                    continue;
                }

                if (GUILayout.Button(node.name, EditorStyles.linkLabel, GUILayout.ExpandWidth(false)))
                {
                    JumpToBreadcrumb(i);
                }

                if (i < path.Count - 1)
                {
                    GUIStyle arrowStyle = new GUIStyle(EditorStyles.label);
                    arrowStyle.normal.textColor = Color.grey;
                    GUILayout.Label(">", GUILayout.Width(10));
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void JumpToBreadcrumb(int target)
        {
            List<UnityObject> path = GetNavigationPath();

            if (target < 0 || target >= path.Count)
            {
                return;
            }
            navigationStack.Clear();

            for (int i = 0; i < target; i++)
            {
                navigationStack.Push(path[i]);
            }
            currentNode = path[target];
            selectedNode = currentNode;
        }

        private void DrawChildrenList()
        {
            if (currentNode == null)
            {
                return;
            }
            INodeContainer container = currentNode as INodeContainer;

            if (container == null)
            {
                EditorGUILayout.HelpBox("This Action cannot have another Action.", MessageType.Info);
                return;
            }
            ValidateActionList();

            childrenSerializedObject.Update();
            childrenList.DoLayoutList();
            childrenSerializedObject.ApplyModifiedProperties();
        }

        private void ValidateActionList()
        {
            if (childrenSerializedObject != null && childrenSerializedObject.targetObject == currentNode)
            {
                return;
            }
            childrenSerializedObject = new SerializedObject(currentNode);
            SerializedProperty property = childrenSerializedObject.FindProperty("objectiveActions");

            if (property == null)
            {
                property = childrenSerializedObject.FindProperty("objectiveSubactions");
            }

            childrenList = new ReorderableList(childrenSerializedObject, property, true, true, true, true);
            childrenList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Actions");
            };

            childrenList.drawElementCallback = (rect, index, active, focused) =>
            {                
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                ObjectiveAction node = element.objectReferenceValue as ObjectiveAction;

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                float rectWidth = rect.width;

                Rect selectRect = new Rect(rect.x, rect.y, 24, rect.height);
                Rect objectRect = new Rect(rect.x + 44, rect.y, rectWidth - 112, rect.height);
                Rect openRect = new Rect(rect.x + rectWidth - 60, rect.y, 30, rect.height);
                Rect pingRect = new Rect(rect.x + rectWidth - 30, rect.y, 30, rect.height);

                if (selectedNode == node)
                {
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.5f, 0.2f, 0.25f));
                }

                if (node != null)
                {
                    if (GUI.Button(selectRect, "S"))
                    {
                        SelectAction(node);
                    }

                    string newName = EditorGUI.TextField(objectRect, node.name);

                    if (newName != node.name)
                    {
                        RenameObject(node, newName);
                    }

                    bool clickableRow = Event.current.type == EventType.MouseDown && 
                        rect.Contains(Event.current.mousePosition) &&
                        !objectRect.Contains(Event.current.mousePosition) &&
                        !openRect.Contains(Event.current.mousePosition) &&
                        !pingRect.Contains(Event.current.mousePosition);
                

                    if (clickableRow)
                    {
                        SelectAction(node);
                        //GUI.FocusControl(null);
                        if (Event.current.clickCount == 2 && node is INodeContainer)
                        {
                            NavigateInto(node);
                        }
                        Event.current.Use();
                    }
                    if (node is INodeContainer)
                    {
                        if (GUI.Button(openRect, "+"))
                        {
                            NavigateInto(node);
                        }
                    }

                    if (GUI.Button(pingRect, "Ping"))
                    {
                        EditorGUIUtility.PingObject(node.gameObject);
                    }
                }
            };

            childrenList.onAddDropdownCallback = (rect, list) =>
            {
                ShowAddMenu();
            };

            childrenList.onRemoveCallback = list =>
            {
                SerializedProperty property = list.serializedProperty;

                if (list.index < 0 || list.index >= property.arraySize)
                {
                    return;
                }
                SerializedProperty element = property.GetArrayElementAtIndex(list.index);
                ObjectiveAction node = element.objectReferenceValue as ObjectiveAction;

                bool confirm = EditorUtility.DisplayDialog("Remove Action", node != null ?
                    $"Remove '{node.name}?" : "Remove this element?", "Remove", "Cancel");

                if (!confirm)
                {
                    return;
                }

                actionToDelete = node;
                nodeParent = currentNode;
                nodeIndexToDelete = list.index;
            };

            
        }
        #endregion

        #region Handle drawing the right panel which functions like the inspector
        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Configure", EditorStyles.boldLabel);

            rightColumnScroll = EditorGUILayout.BeginScrollView(rightColumnScroll);

            if (selectedNode != null)
            {
                if (cachedEditor == null || cachedEditor.target != selectedNode)
                {
                    //safety check for the cached editor before creating one
                    if (cachedEditor != null)
                    {
                        DestroyImmediate(cachedEditor);
                    }
                    UnityEditor.Editor.CreateCachedEditor(selectedNode, null, ref cachedEditor);
                }
                cachedEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Handle navigating the interface
        private void SelectGoal(Goal goal)
        {
            selectedGoal = goal;
            currentNode = goal;
            selectedNode = goal;
            navigationStack.Clear();
        }

        private void SelectAction(UnityObject node)
        {
            selectedNode = node;
            Selection.activeObject = node;
        }
        
        private void NavigateInto(UnityObject node)
        {
            navigationStack.Push(currentNode);
            currentNode = node;
            selectedNode = node;
        }

        private void NavigateBack()
        {
            if (navigationStack.Count > 0)
            {
                currentNode = navigationStack.Pop();
                selectedNode = currentNode;
            }
        }

        private List<UnityObject> GetNavigationPath()
        {
            List<UnityObject> path = new List<UnityObject>();

            UnityObject[] stackItems = navigationStack.ToArray();
            Array.Reverse(stackItems);

            path.AddRange(stackItems);

            if (currentNode != null)
            {
                path.Add(currentNode);
            }
            return path;
        }
        #endregion

        #region Handle adding nodes
        private void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<ObjectiveAction>();
            List<Type> validTypes = new List<Type>();

            if (!typeof(ObjectiveAction).IsAbstract)
            {
                validTypes.Add(typeof(ObjectiveAction));
            }

            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (!validTypes.Contains(type))
                { 
                    validTypes.Add(type);
                }
            }

            validTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            foreach (Type type in validTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () => CreateNode(type));
            }
            menu.ShowAsContext();
        }

        private void CreateNode(Type type)
        {
            if (!(currentNode is Component parent))
            {
                return;
            }

            GameObject newObject = new GameObject(type.Name);
            newObject.transform.SetParent(parent.transform);

            ObjectiveAction newNode = (ObjectiveAction)newObject.AddComponent(type);
            SerializedObject newSerializedObject = new SerializedObject(currentNode);
            SerializedProperty property = newSerializedObject.FindProperty("objectiveActions") ??
                newSerializedObject.FindProperty("objectiveSubactions");
            
            int propIndex = property.arraySize;
            
            property.InsertArrayElementAtIndex(propIndex);
            property.GetArrayElementAtIndex(propIndex).objectReferenceValue = newNode;

            newSerializedObject.ApplyModifiedProperties();
            selectedNode = newNode;
        }

        private void ShowCreateGoalMenu()
        {
            GenericMenu menu = new GenericMenu();

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Goal>();
            List<Type> validTypes = new List<Type>();

            if (!typeof(Goal).IsAbstract)
            {
                validTypes.Add(typeof(Goal));
            }

            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                if (!validTypes.Contains(type))
                {
                    validTypes.Add(type);
                }
            }
            validTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            if (validTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Goal types found"));
            }
            else
            {
                foreach (Type type in validTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () => CreateGoal(type));
                }
            }
            menu.ShowAsContext();
        }

        private void CreateGoal(Type type)
        {
            GameObject newObject = new GameObject(type.Name);
            Undo.RegisterCreatedObjectUndo(newObject, "Create Goal");
            Goal newGoal = (Goal)newObject.AddComponent(type);

            SelectGoal(newGoal);

            goalListNeedsRefresh = true;
            Selection.activeGameObject = newObject;
            EditorGUIUtility.PingObject(newObject);
        }

        private void RenameObject(UnityObject targetObject, string newName)
        {
            if (targetObject is Component comp)
            {
                Undo.RecordObject(comp.gameObject, "Rename Node");
                comp.gameObject.name = newName;
                EditorUtility.SetDirty(comp.gameObject); 
            }
        }
        #endregion


        #region Handle deleting the Goals/Actions
        private void HandleDeferredDeletes()
        {
            if (goalToDelete != null)
            {
                DeleteGoal(goalToDelete);
                goalToDelete = null;
                GUIUtility.ExitGUI();
            }
            if (nodeIndexToDelete != -1 && nodeParent != null)
            {
                SerializedObject newNodeParent = new SerializedObject(nodeParent);
                SerializedProperty property = newNodeParent.FindProperty("objectiveActions") ?? newNodeParent.FindProperty("objectiveSubactions");

                if (property != null && nodeIndexToDelete < property.arraySize)
                {
                    property.DeleteArrayElementAtIndex(nodeIndexToDelete);
                    newNodeParent.ApplyModifiedProperties();
                }
                if (actionToDelete != null)
                {
                    if (selectedNode == actionToDelete)
                    {
                        selectedNode = currentNode;
                    }
                    Undo.DestroyObjectImmediate(actionToDelete.gameObject);
                }
                actionToDelete = null;
                nodeParent = null;
                nodeIndexToDelete = -1;

                GUIUtility.ExitGUI();
            }
        }

        private void DeleteGoal(Goal targetGoal)
        {
            if (targetGoal == null)
            {
                return;
            }

            bool confirm = EditorUtility.DisplayDialog("Delete Goal", 
                $"Delete '{targetGoal.name} and all its child objects?", "Delete", "Cancel");
            
            if (!confirm)
            {
                return;
            }

            if (selectedGoal == targetGoal)
            {
                selectedGoal = null;
                currentNode = null;
                selectedNode = null;
                navigationStack.Clear();
            }
            Undo.DestroyObjectImmediate(targetGoal.gameObject);
            goalListNeedsRefresh = true;
        }
        #endregion

        private void OnHierarchyChange()
        {
            goalListNeedsRefresh = true;
            Repaint();
        }
    }
}
#endif