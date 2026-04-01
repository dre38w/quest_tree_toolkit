using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TestWindow : EditorWindow
{
    GameObject testSceneObject;
    Editor componentEditor;

    List<GameObject> testObjectChildren = new List<GameObject>();

    [MenuItem("Tools/Quest Tree Tool")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(TestWindow), false);
        window.maxSize = new Vector2(300, 300);
        window.minSize = window.maxSize;
    }


    private void OnGUI()
    {
        testSceneObject = (GameObject)EditorGUILayout.ObjectField(testSceneObject, typeof(GameObject), true);

        if (testSceneObject ==  null)
        {
            return;
        }

        foreach (Transform obj in testSceneObject.GetComponentsInChildren<Transform>())
        {
            testObjectChildren.Add(obj.gameObject);
        }
        //will use scripts in place of transform
        var component = testSceneObject.GetComponent<Transform>();

        if (component == null)
        {
            EditorGUILayout.LabelField("Component not found.");
            return;
        }

        if (componentEditor == null || componentEditor.target != component)
        {
            componentEditor = Editor.CreateEditor(component);
        }

        if (componentEditor != null)
        {
            componentEditor.OnInspectorGUI();
        }
    }
}
