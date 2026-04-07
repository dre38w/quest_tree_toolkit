#if UNITY_EDITOR
using Service.Framework.Goals;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Goal))]
public class GoalEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    if (SerializationUtility.HasManagedReferencesWithMissingTypes(target))
    //    {
    //        EditorGUILayout.HelpBox("This object is missing SeralizeReference types.", MessageType.Error);

    //        if (GUILayout.Button("Fix Missing References"))
    //        {
    //            Undo.RecordObject(target, "Fix Missing Managed References");

    //            SerializationUtility.ClearAllManagedReferencesWithMissingTypes(target);
    //            EditorUtility.SetDirty(target);
    //            serializedObject.ApplyModifiedProperties();
    //        }
    //        GUIUtility.ExitGUI();
    //    }
    //    DrawDefaultInspector();
    //    serializedObject.ApplyModifiedProperties();
    //}
}
#endif