/*
 * Description: A custom Editor UI that handles creating and organizing the Goal and Objective Actions
 */
#if UNITY_EDITOR
using Service.Core;
using Service.Framework;
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
        private const string PROPERTY_ACTIONS_NAME = "objectiveActions";
        private const string PROPERTY_SUBACTIONS_NAME = "objectiveSubactions";
        
        //default width of the panels
        private const float MIN_LEFT_PANEL_WIDTH = 180f;
        private const float MIN_RIGHT_PANEL_WIDTH = 260f;
        private const float MIN_CENTER_PANEL_WIDTH = 240f;
        //width of the splitters that separate the panels
        private const float SPLITTER_WIDTH = 4f;
        private const int MAX_BREADCRUMBS_VISIBLE = 4;

        //the adjustable width of the left and right panels
        private float leftWidth = 220f;
        private float rightWidth = 320f;

        private bool isDraggingLeftSplitter;
        private bool isDraggingRightSplitter;

        //used to indicate when a redraw is required
        //TODO:  may not need this
        private bool isRebuildGoalList = true;

        private Vector2 leftPanelScroll;
        private Vector2 centerPanelScroll;
        private Vector2 rightPanelScroll;

        private UnityObject currentNode;
        private UnityObject selectedNode;

        private UnityEditor.Editor cachedEditor;
        private ReorderableList nestedActionsList;
        private SerializedObject nestedActionsSerializedObject;

        private Goal selectedGoal;

        private Goal goalToDelete;
        private ObjectiveAction actionToDelete;
        private UnityObject nodeParent;
        private int nodeIndexToDelete = -1;

        private GUIStyle breadcrumbArrowStyle;

        private ReorderableList goalList;
        private List<Goal> cachedGoals = new List<Goal>();

        //holds a history of where we were in the stack
        private readonly Stack<UnityObject> navigationStack = new Stack<UnityObject>();

        /// <summary>
        /// Create the window
        /// </summary>
        [MenuItem("Tools/Quest Tree Editor")]
        public static void ShowWindow()
        {
            GetWindow<QuestTreeEditorWindow>("Quest Tree Editor");
        }

        private void OnGUI()
        {
            //clamp the window to be no smaller than the sum of the panels' widths
            //this will prevent the panels from overlapping each other if resized too small
            minSize = new Vector2(MIN_LEFT_PANEL_WIDTH + MIN_CENTER_PANEL_WIDTH + MIN_RIGHT_PANEL_WIDTH + (SPLITTER_WIDTH * 2f), 300f);

            HandleSplitters();
            HandleResizingPanels();

            float windowWidth = position.width;
            float windowHeight = position.height;

            //handle drawing the window rects
            Rect leftRect = new Rect(0, 0, leftWidth, windowHeight);
            Rect leftSplitterRect = new Rect(leftRect.xMax, 0, SPLITTER_WIDTH, windowHeight);

            Rect rightRect = new Rect(windowWidth - rightWidth, 0, rightWidth, windowHeight);
            Rect rightSplitterRect = new Rect(rightRect.xMin - SPLITTER_WIDTH, 0, SPLITTER_WIDTH, windowHeight);

            Rect centerRect = new Rect(leftSplitterRect.xMax, 0f, rightSplitterRect.xMin - leftSplitterRect.xMax, windowHeight);

            //draw left panel
            GUILayout.BeginArea(leftRect);
            DrawLeftPanel();
            GUILayout.EndArea();

            //draw center panel
            GUILayout.BeginArea(centerRect);
            DrawCenterPanel();
            GUILayout.EndArea();

            //draw right panel
            GUILayout.BeginArea(rightRect);
            DrawRightPanel();
            GUILayout.EndArea();

            DrawSplitter(leftSplitterRect);
            DrawSplitter(rightSplitterRect);

            HandleDeferredDeletes();
        }

        #region Handle Window
        /// <summary>
        /// Handle dragging the splitters
        /// </summary>
        private void HandleSplitters()
        {
            Event currentEvent = Event.current;

            Rect leftSplitterRect = new Rect(leftWidth, 0f, SPLITTER_WIDTH, position.height);
            Rect rightSplitterRect = new Rect(position.width - rightWidth - SPLITTER_WIDTH, 0f, SPLITTER_WIDTH, position.height);

            EditorGUIUtility.AddCursorRect(leftSplitterRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(rightSplitterRect, MouseCursor.ResizeHorizontal);

            switch (currentEvent.type)
            {
                //select left or right splitter
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
                    //drag left or right splitter
                case EventType.MouseDrag:
                    //resize the left panel
                    if (isDraggingLeftSplitter)
                    {
                        //update the width that we'll use to resize the panel
                        leftWidth = currentEvent.mousePosition.x;
                        //resize the panel now that we're dragging the splitter
                        HandleResizingPanels();
                        //repaint the UI to visually update it
                        Repaint();
                        currentEvent.Use();
                    }
                    //resize the right panel
                    else if (isDraggingRightSplitter)
                    {
                        rightWidth = position.width - currentEvent.mousePosition.x - SPLITTER_WIDTH;
                        HandleResizingPanels();
                        Repaint();
                        currentEvent.Use();
                    }
                    break;
                    //release splitters
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

        /// <summary>
        /// Draw the splitters
        /// </summary>
        /// <param name="rect"></param>
        private void DrawSplitter(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f, 1));
        }

        /// <summary>
        /// Calculate the panel size when user resizes them via dragging the splitters
        /// </summary>
        private void HandleResizingPanels()
        {
            float totalWidth = position.width;
            float availableSpaceForPanels = totalWidth - (SPLITTER_WIDTH * 2f);

            float maxLeft = availableSpaceForPanels - MIN_CENTER_PANEL_WIDTH - MIN_RIGHT_PANEL_WIDTH;
            float maxRight = availableSpaceForPanels - MIN_CENTER_PANEL_WIDTH - MIN_LEFT_PANEL_WIDTH;

            maxLeft = Mathf.Max(MIN_LEFT_PANEL_WIDTH, maxLeft);
            maxRight = Mathf.Max(MIN_RIGHT_PANEL_WIDTH, maxRight);

            leftWidth = Mathf.Clamp(leftWidth, MIN_LEFT_PANEL_WIDTH, maxLeft);
            rightWidth = Mathf.Clamp(rightWidth, MIN_RIGHT_PANEL_WIDTH, maxRight);

            //the amount of space the center panel has remaining if user is resizing the window
            float centerWidth = availableSpaceForPanels - leftWidth - rightWidth;

            if (centerWidth < MIN_CENTER_PANEL_WIDTH)
            {
                //the amount of space the center panel is missing.
                //this is used for layout correction when resizing the window
                float centerWidthNeeded = MIN_CENTER_PANEL_WIDTH - centerWidth;

                //do the layout correction when resizing the right panel:
                //shrink the larger side first for a more natural feel while resizing
                if (rightWidth > leftWidth)
                {
                    float newRight = Mathf.Max(MIN_RIGHT_PANEL_WIDTH, rightWidth - centerWidthNeeded);
                    //the amount of layout correction we made
                    centerWidthNeeded -= rightWidth - newRight;
                    rightWidth = newRight;

                    //if the right panel couldn't correct the layout, use the left panel to correct it
                    if (centerWidthNeeded > 0f)
                    {
                        leftWidth = Mathf.Max(MIN_LEFT_PANEL_WIDTH, leftWidth - centerWidthNeeded);
                    }
                }
                //do layout correction when resizing the left panel
                else
                {
                    float newLeft = Mathf.Max(MIN_LEFT_PANEL_WIDTH, leftWidth - centerWidthNeeded);
                    centerWidthNeeded -= leftWidth - newLeft;
                    leftWidth = newLeft;

                    if (centerWidthNeeded > 0f)
                    {
                        rightWidth = Mathf.Max(MIN_RIGHT_PANEL_WIDTH, rightWidth - centerWidthNeeded);
                    }
                }
            }
        }
        #endregion

        #region Configure and manage left panel and the list of Goals
        /// <summary>
        /// Left panel contains a list of all the Goals in the scene
        /// </summary>
        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Goals", EditorStyles.boldLabel);

            HandleGoalList();

            leftPanelScroll = EditorGUILayout.BeginScrollView(leftPanelScroll);

            //have the list do the list things
            if (goalList != null)
            {
                goalList.DoLayoutList();
                SyncGoalSelection();
            }
            EditorGUILayout.EndScrollView();

            //create a button to add a new goal
            if (GUILayout.Button("Create New Goal"))
            {
                ShowCreateGoalMenu();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw and handle input for the goal list
        /// </summary>
        private void HandleGoalList()
        {
            if (!isRebuildGoalList && goalList != null)
            {
                return;
            }
            //clear previous
            cachedGoals.Clear();
            //add all those in the hierarchy
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

            goalList.index = -1;
            if (selectedGoal != null)
            {
                for (int i = 0; i < cachedGoals.Count; i++)
                {
                    if (cachedGoals[i] == selectedGoal)
                    {
                        goalList.index = i;
                        break;
                    }
                }
            }

            goalList.onSelectCallback = list =>
            {
                SyncGoalSelection();
            };

            goalList.drawElementCallback = (rect, index, active, focused) =>
            {
                Goal goal = cachedGoals[index];
                if (goal == null)
                {
                    return;
                }
                //create the rows by adding a value to the previous value
                //then we'll apply this to the rects
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;

                float width = rect.width;
                //the goal's name field
                Rect nameRect = new Rect(rect.x, rect.y, width - 120f, rect.height);
                //the select goal button
                Rect selectRect = new Rect(rect.x + width - 115f, rect.y, 55f, rect.height);
                //the delete goal button
                Rect deleteRect = new Rect(rect.x + width - 55f, rect.y, 50f, rect.height);

                //allow renaming of the Goal
                string newName = EditorGUI.TextField(nameRect, goal.name);

                if (newName != goal.name)
                {
                    RenameObject(goal, newName);
                }

                //select the goal to show its contents in the center panel
                if (GUI.Button(selectRect, "Select"))
                {
                    SelectGoal(goal);
                }
                //delete the goal from the hierarchy and from the tool
                if (GUI.Button(deleteRect, "Delete"))
                {
                    //mark it for deletion
                    goalToDelete = goal;
                }
            };

            //callback for dragging and reordering the goals in the tool
            goalList.onReorderCallback = list =>
            {
                for (int i = 0; i < cachedGoals.Count; i++)
                {
                    if (cachedGoals[i] != null)
                    {
                        Undo.RecordObject(cachedGoals[i].transform, "Reorder Goals");
                        //apply changes to the moved goal
                        cachedGoals[i].transform.SetSiblingIndex(i);
                    }
                }
                isRebuildGoalList = true;
            };
            isRebuildGoalList = false;
        }
        #endregion



        #region Handle drawing the center panel and manage the Actions in it
        /// <summary>
        /// Draw the center panel
        /// </summary>
        private void DrawCenterPanel()
        {
            EditorGUILayout.BeginVertical();

            //display an info message informing the user to select a goal
            //if no goal is selected, then no content is shown
            if (selectedGoal == null)
            {
                EditorGUILayout.HelpBox("Select a Goal", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }
            DrawHeader();

            //create a scroll view in the event we have a long list of actions
            centerPanelScroll = EditorGUILayout.BeginScrollView(centerPanelScroll);
            DrawNestedActionsList();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw the center panel's header
        /// </summary>
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
                    //create new guistyle if one doesn't exist
                    if (breadcrumbArrowStyle == null)
                    {
                        breadcrumbArrowStyle = new GUIStyle(EditorStyles.label);
                        breadcrumbArrowStyle.normal.textColor = Color.grey;
                    }
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

        /// <summary>
        /// Draw the nested actions.  Nested actions are child actions of other actions
        /// </summary>
        private void DrawNestedActionsList()
        {
            if (currentNode == null)
            {
                return;
            }
            //get a reference to the current action using its interface
            INodeContainer container = currentNode as INodeContainer;

            //this action does not have this interface and therefore it cannot contain a child action
            if (container == null)
            {
                EditorGUILayout.HelpBox("This Action cannot have another Action added to it.", MessageType.Info);
                return;
            }
            //validate and draw
            HandleActionsList();

            //update the new object and do the list things
            nestedActionsSerializedObject.Update();
            nestedActionsList.DoLayoutList();
            nestedActionsSerializedObject.ApplyModifiedProperties();

            SyncActionSelection();
        }

        /// <summary>
        /// Draw and handle input for the actions
        /// </summary>
        private void HandleActionsList()
        {
            if (nestedActionsSerializedObject != null && nestedActionsSerializedObject.targetObject == currentNode)
            {
                return;
            }
            nestedActionsSerializedObject = new SerializedObject(currentNode);
            SerializedProperty property = GetNestedActionsProperty(nestedActionsSerializedObject);

            //make them a reorderable list
            nestedActionsList = new ReorderableList(nestedActionsSerializedObject, property, true, true, true, true);

            nestedActionsList.index = -1;

            if (selectedNode != null)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    if (property.GetArrayElementAtIndex(i).objectReferenceValue == selectedNode)
                    {
                        nestedActionsList.index = i;
                        break;
                    }
                }
            }

            nestedActionsList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Actions");
            };

            nestedActionsList.onSelectCallback = list =>
            {
                SyncActionSelection();
            };

            //callback to draw the list onto the tool UI
            nestedActionsList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                ObjectiveAction node = element.objectReferenceValue as ObjectiveAction;

                //create some rows
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                float rectWidth = rect.width;

                //select button's rect
                Rect selectRect = new Rect(rect.x, rect.y, 24, rect.height);
                //text field's rect
                Rect objectRect = new Rect(rect.x + 44, rect.y, rectWidth - 175, rect.height);
                //duplicate button
                Rect duplicateRect = new Rect(rect.x + rectWidth - 120, rect.y, 30, rect.height);
                //'+' button's rect
                Rect openRect = new Rect(rect.x + rectWidth - 80, rect.y, 30, rect.height);
                //ping button
                Rect pingRect = new Rect(rect.x + rectWidth - 40, rect.y, 40, rect.height);

                //highlight the selection
                if (nestedActionsList != null && nestedActionsList.index == index)
                {
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.5f, 0.2f, 0.75f));
                }

                if (node != null)
                {
                    //select the action
                    if (GUI.Button(selectRect, "S"))
                    {
                        SelectAction(node);
                    }

                    //create the text field and allow rename
                    string newName = EditorGUI.TextField(objectRect, node.name);

                    //if its not equal to the class's name that means we renamed it
                    if (newName != node.name)
                    {
                        RenameObject(node, newName);
                    }

                    //duplicate the entry
                    if (GUI.Button(duplicateRect, "D"))
                    {
                        DuplicateNode(node);
                    }

                    //create the space we're allowed to double click on
                    bool clickableRow = Event.current.type == EventType.MouseDown &&
                        Event.current.clickCount == 2 &&
                        rect.Contains(Event.current.mousePosition) &&
                        !objectRect.Contains(Event.current.mousePosition) &&
                        !openRect.Contains(Event.current.mousePosition) &&
                        !pingRect.Contains(Event.current.mousePosition) &&
                        !duplicateRect.Contains(Event.current.mousePosition);

                    if (node is INodeContainer)
                    {
                        if (clickableRow)
                        {
                            NavigateInto(node);
                            Event.current.Use();
                        }
                    }
                    //check that this entry can have actions nested in it
                    if (node is INodeContainer)
                    {
                        //use the + button to navigate in
                        if (GUI.Button(openRect, "+"))
                        {
                            NavigateInto(node);
                        }
                    }
                    //ping the object in the hierarchy
                    if (GUI.Button(pingRect, "Ping"))
                    {
                        EditorGUIUtility.PingObject(node.gameObject);
                    }
                }
            };

            nestedActionsList.onAddDropdownCallback = (rect, list) =>
            {
                ShowAddMenu();
            };

            //callback when trying to delete an action
            nestedActionsList.onRemoveCallback = list =>
            {
                SerializedProperty property = list.serializedProperty;

                if (list.index < 0 || list.index >= property.arraySize)
                {
                    return;
                }
                //get the one we selected
                SerializedProperty element = property.GetArrayElementAtIndex(list.index);
                ObjectiveAction node = element.objectReferenceValue as ObjectiveAction;

                //display a confirmation popup window
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

        private void DuplicateNode(ObjectiveAction sourceNode)
        {
            if (sourceNode == null)
            {
                return;
            }

            if (currentNode is not Component parent)
            {
                Debug.LogError("Current node is not a valid parent.");
                return;
            }

            //copy by instantiating the object we are selected on
            GameObject clone = Instantiate(sourceNode.gameObject, parent.transform);
            clone.name = sourceNode.name + "_Clone";

            //allow undo
            Undo.RegisterCreatedObjectUndo(clone, "Duplicate Node");
            ObjectiveAction newNode = clone.GetComponent<ObjectiveAction>();

            SerializedObject newSerializedObject = new SerializedObject(currentNode);
            SerializedProperty property = GetNestedActionsProperty(newSerializedObject);

            //place it in the list array
            int propertyIndex = property.arraySize;
            property.InsertArrayElementAtIndex(propertyIndex);
            property.GetArrayElementAtIndex(propertyIndex).objectReferenceValue = newNode;

            newSerializedObject.ApplyModifiedProperties();

            //select the new copy
            nestedActionsList.index = propertyIndex;
            selectedNode = newNode;
            Selection.activeObject = newNode;

            //ping the game object in the hierarchy for visual clarity
            EditorGUIUtility.PingObject(newNode.gameObject);
        }
        #endregion

        #region Handle drawing the right panel which functions like the inspector
        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Configure Settings", EditorStyles.boldLabel);

            rightPanelScroll = EditorGUILayout.BeginScrollView(rightPanelScroll);

            //make sure something is selected before trying to draw its component on the panel
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
                //the right panel functions very similar to Unity's inspector
                //it only displays the ObjectiveActions and Goal components
                if (cachedEditor.serializedObject != null)
                {
                    cachedEditor.serializedObject.Update();
                    //view as inspector
                    cachedEditor.OnInspectorGUI();
                    cachedEditor.serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Handle navigating the interface
        /// <summary>
        /// Set the selected goal
        /// </summary>
        /// <param name="goal"></param>
        private void SelectGoal(Goal goal)
        {
            if (goal == null)
            {
                return;
            }

            selectedGoal = goal;
            currentNode = goal;
            selectedNode = goal;
            //clear the navigation stack because this is the beginning of our navigation tree
            //this avoids navigating back to a previous goal selection
            navigationStack.Clear();

            if (goalList != null)
            {
                goalList.index = -1;

                for (int i = 0; i < cachedGoals.Count; i++)
                {
                    if (cachedGoals[i] == goal)
                    {
                        goalList.index = i;
                        break;
                    }
                }
            }
            nestedActionsSerializedObject = null;
            nestedActionsList = null;
            Repaint();
        }
                
        /// <summary>
        /// Used to auto select objects and set a selection as the active object in our list
        /// Primarily used when requiring a selection without the user's input
        /// or when utilizing the custom input rather than Unity's.  (buttons, rect based selection, etc)
        /// </summary>
        /// <param name="node"></param>
        private void SelectAction(UnityObject node)
        {
            selectedNode = node;
            //highlight the selected object in the hierarchy
            Selection.activeObject = node;

            if (nestedActionsList != null)
            {
                //get the list of actions
                SerializedProperty property = nestedActionsList.serializedProperty;
                //clear the list indices to avoid pointing to a null index
                nestedActionsList.index = -1;

                for (int i = 0; i < property.arraySize; i++)
                {
                    //find the index by the provided node reference
                    if (property.GetArrayElementAtIndex(i).objectReferenceValue == node)
                    {
                        //apply the index
                        nestedActionsList.index = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sync to Unity's global internal selection so the selected element in the list
        /// is accurate to what Unity is registering as a selection
        /// User selects the row rather than a rect, button, etc, so update to match that
        /// </summary>
        private void SyncActionSelection()
        {
            if (nestedActionsList == null)
            {
                return;
            }

            SerializedProperty property = nestedActionsList.serializedProperty;
            //get the selected index
            int index = nestedActionsList.index;

            if (index >= 0 && index < property.arraySize)
            {
                //get the referenced object
                UnityObject node = property.GetArrayElementAtIndex(index).objectReferenceValue;
                //set the selected node to that internal selection
                selectedNode = node;
                Repaint();
            }
        }

        private void SyncGoalSelection()
        {
            if (goalList == null)
            {
                return;
            }

            int index = goalList.index;

            if (index >= 0 && index < cachedGoals.Count)
            {
                Goal goal = cachedGoals[index];

                if (goal == null)
                {
                    return;
                }

                if (selectedGoal == goal)
                {
                    return;
                }

                selectedGoal = goal;
                currentNode = goal;
                selectedNode = goal;
                navigationStack.Clear();

                nestedActionsSerializedObject = null;
                nestedActionsList = null;
                Repaint();
            }
        }

        /// <summary>
        /// Navigate to a specific layer in the breadcrumb
        /// </summary>
        /// <param name="node"></param>
        private void NavigateInto(UnityObject node)
        {
            //navigate to it
            navigationStack.Push(currentNode);
            currentNode = node;
            selectedNode = node;
        }

        /// <summary>
        /// Go back one layer
        /// </summary>
        private void NavigateBack()
        {
            if (navigationStack.Count > 0)
            {
                //go back to the previous one
                currentNode = navigationStack.Pop();
                selectedNode = currentNode;
            }
        }

        /// <summary>
        /// Get the navigation path that
        /// </summary>
        /// <returns></returns>
        private List<UnityObject> GetNavigationPath()
        {
            List<UnityObject> path = new List<UnityObject>();

            //returns the reverse order of how we want to navigate
            //returns the most recent at the bottom of the list
            UnityObject[] stackItems = navigationStack.ToArray();
            //reverse it so we can properly navigate backwards
            //having the most recent added to the stack as the first in the array
            Array.Reverse(stackItems);
            
            path.AddRange(stackItems);

            //add the current node to the array since the stack does not hold history to the current entry, only previous entries
            if (currentNode != null)
            {
                path.Add(currentNode);
            }
            return path;
        }
        #endregion

        #region Handle adding nodes
        /// <summary>
        /// Show the create actions menu
        /// </summary>
        private void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            //show and select an entry in the menu
            foreach (Type type in GetValidTypes<ObjectiveAction>())
            {
                string path = GetMenuPath(type);
                menu.AddItem(new GUIContent(path), false, () => CreateNode(type));
            }
            menu.ShowAsContext();
        }

        private string GetMenuPath(Type type)
        {
            SubmenuAttribute menuAttribute = (SubmenuAttribute)Attribute.GetCustomAttribute(type, typeof(SubmenuAttribute));

            if (menuAttribute != null && !string.IsNullOrEmpty(menuAttribute.Path))
            {
                return menuAttribute.Path;
            }
            return "Other/" + type.Name;
        }

        /// <summary>
        /// Handle creating the new action
        /// </summary>
        /// <param name="type"></param>
        private void CreateNode(Type type)
        {
            if (!(currentNode is Component parent))
            {
                return;
            }

            //create the object in the hiearchy
            GameObject newObject = new GameObject(type.Name);
            //set its parent to be the object that holds the list this entry is being added to
            newObject.transform.SetParent(parent.transform);

            //now add the chosen class component
            ObjectiveAction newNode = (ObjectiveAction)newObject.AddComponent(type);
            SerializedObject newSerializedObject = new SerializedObject(currentNode);
            SerializedProperty property = GetNestedActionsProperty(newSerializedObject);

            int propIndex = property.arraySize;

            //place them in the tool list
            property.InsertArrayElementAtIndex(propIndex);
            property.GetArrayElementAtIndex(propIndex).objectReferenceValue = newNode;

            newSerializedObject.ApplyModifiedProperties();

            //and set it to be the selected entry
            SelectAction(newNode);
        }

        /// <summary>
        /// Show the create goal menu
        /// </summary>
        private void ShowCreateGoalMenu()
        {
            GenericMenu menu = new GenericMenu();
            List<Type> validTypes = GetValidTypes<Goal>();

            //ensure we have goals created to add
            if (validTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Goal types found"));
            }
            else
            {
                //go through all of them and then add them to the list
                foreach (Type type in validTypes)
                {
                    //the callback that allows us to call CreateGoal when clicking an item in the menu
                    menu.AddItem(new GUIContent(type.Name), false, () => CreateGoal(type));
                }
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// Add the new goal to the game
        /// </summary>
        /// <param name="type"></param>
        private void CreateGoal(Type type)
        {
            //create the game object in the hierarchy
            GameObject newObject = new GameObject(type.Name);
            //allow undo
            Undo.RegisterCreatedObjectUndo(newObject, "Create Goal");
            //add the chosen goal component
            Goal newGoal = (Goal)newObject.AddComponent(type);

            SelectGoal(newGoal);

            isRebuildGoalList = true;
            Selection.activeGameObject = newObject;
            EditorGUIUtility.PingObject(newObject);
        }

        /// <summary>
        /// Apply the new name
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="newName"></param>
        private void RenameObject(UnityObject targetObject, string newName)
        {
            if (targetObject is Component comp)
            {
                //allow undo
                Undo.RecordObject(comp.gameObject, "Rename Node");
                //apply the new name to the game object in the hierarchy
                comp.gameObject.name = newName;
                EditorUtility.SetDirty(comp.gameObject);
            }
        }
        #endregion


        #region Handle deleting the Goals/Actions
        /// <summary>
        /// Handle the goals or actions that are being marked for deletion
        /// </summary>
        private void HandleDeferredDeletes()
        {
            //if this is a goal we're deleting
            if (goalToDelete != null)
            {
                DeleteGoal(goalToDelete);
                //make sure this is now clear for the next one
                goalToDelete = null;
                //avoid running anything else
                GUIUtility.ExitGUI();
            }

            //if this is an objective action to delete
            if (nodeIndexToDelete != -1 && nodeParent != null)
            {
                //get the type of action
                SerializedObject newNodeParent = new SerializedObject(nodeParent);
                SerializedProperty property = GetNestedActionsProperty(newNodeParent);
                                
                if (property != null && nodeIndexToDelete < property.arraySize)
                {
                    //delete it from the tool
                    property.DeleteArrayElementAtIndex(nodeIndexToDelete);
                    newNodeParent.ApplyModifiedProperties();

                    //allow undo and destroy the gameobject
                    if (actionToDelete != null)
                    {
                        Undo.DestroyObjectImmediate(actionToDelete.gameObject);
                    }
                    //go to the parent node
                    selectedNode = currentNode;
                    Selection.activeObject = currentNode;

                    //avoid pointing to a null entry
                    if (nestedActionsList != null)
                    {
                        nestedActionsList.index = -1;
                    }
                    //refresh the UI
                    Repaint();
                }
                //reset the values
                actionToDelete = null;
                nodeParent = null;
                nodeIndexToDelete = -1;

                //exit
                GUIUtility.ExitGUI();
            }
        }

        /// <summary>
        /// Delete the goal
        /// </summary>
        /// <param name="targetGoal"></param>
        private void DeleteGoal(Goal targetGoal)
        {
            if (targetGoal == null)
            {
                return;
            }

            //display confirmation window
            bool confirm = EditorUtility.DisplayDialog("Delete Goal",
                $"Delete '{targetGoal.name}' and all its child objects?", "Delete", "Cancel");

            if (!confirm)
            {
                return;
            }

            //clear the references
            if (selectedGoal == targetGoal)
            {
                selectedGoal = null;
                currentNode = null;
                selectedNode = null;
                navigationStack.Clear();
            }
            //allow undo and destroy it
            Undo.DestroyObjectImmediate(targetGoal.gameObject);
            isRebuildGoalList = true;
        }
        #endregion

        /// <summary>
        /// If the hiearchy is updated, make sure the tool syncs
        /// </summary>
        private void OnHierarchyChange()
        {
            isRebuildGoalList = true;
            Repaint();
        }

        #region Helper methods
        private List<Type> GetValidTypes<T>()
        {
            List<Type> validTypes = new List<Type>();

            //allow base classes
            if (!typeof(T).IsAbstract)
            {
                validTypes.Add(typeof(T));
            }

            //get derived classes
            foreach (Type type in TypeCache.GetTypesDerivedFrom<T>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                //if it doesn't contain the requested type, add it
                if (!validTypes.Contains(type))
                {
                    validTypes.Add(type);
                }
            }
            //sort them alphabetically
            validTypes.Sort((a, b) => string.Compare(GetMenuPath(a), GetMenuPath(b), StringComparison.Ordinal));

            return validTypes;
        }

        /// <summary>
        /// Get the objective actions property
        /// </summary>
        /// <param name="serializedObject">Pass the object we want to find the property of</param>
        /// <returns>Return either actions or subactions depending upon the object we passed</returns>
        private SerializedProperty GetNestedActionsProperty(SerializedObject serializedObject)
        {
            SerializedProperty actionsProperty = serializedObject.FindProperty(PROPERTY_ACTIONS_NAME);
            SerializedProperty subactionsProperty = serializedObject.FindProperty(PROPERTY_SUBACTIONS_NAME);
            
            return actionsProperty ?? subactionsProperty;
        }

        /// <summary>
        /// Do clean up
        /// </summary>
        private void OnDisable()
        {
            if (cachedEditor != null)
            {
                DestroyImmediate(cachedEditor);
                cachedEditor = null;
            }
        }
        #endregion
    }
}
#endif