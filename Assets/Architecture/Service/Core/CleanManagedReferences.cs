/*
 * Description: Handles cleaning up any broken or missing Managed Serialized References when Unity's domain reloads
 */
#if UNITY_EDITOR
using Service.Framework.Goals;
using UnityEditor;
using UnityEngine;

namespace Service.Core
{
    [InitializeOnLoad]
    public static class CleanManagedReferences
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static CleanManagedReferences()
        {
            //clean up any lingering listeners
            AssemblyReloadEvents.afterAssemblyReload -= RunAfterReload;
            //add a new one to listen for when Unity's domain reloads
            AssemblyReloadEvents.afterAssemblyReload += RunAfterReload;
        }

        private static void RunAfterReload()
        {
            //clean up any lingering listeners
            EditorApplication.delayCall -= Run;
            //delay to allow Unity to finish loading
            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            int fixedCount = 0;

            //iterate over the goal classes in the scene
            foreach (Goal goal in Object.FindObjectsByType<Goal>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (FixObject(goal))
                {
                    //track the number of objects that were fixed
                    fixedCount++;
                }
            }
            //work with the prefab using GUIDs
            string[] guids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in guids)
            {
                //get the actual guid path to point toward the prefab
                string path = AssetDatabase.GUIDToAssetPath(guid);
                //load in the prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                //keep trying until we find one
                if (prefab == null)
                {
                    continue;
                }
                //handle Goals that are children of a prefab
                Goal[] goals = prefab.GetComponentsInChildren<Goal>(true);
                bool prefabChanged = false;

                //attempt to fix any of those if need be
                foreach (Goal goal in goals)
                {
                    if (FixObject(goal))
                    {
                        prefabChanged = true;
                        fixedCount++;
                    }
                }
                //set the updated data
                if (prefabChanged)
                {
                    EditorUtility.SetDirty(prefab);
                }
            }
            //if we fixed anything, save the data to the asset
            if (fixedCount > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log($"Fixed {fixedCount} managed reference issues.");
            }
        }

        /// <summary>
        /// Fix a Goal object that has a missing managed serialized reference
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        private static bool FixObject(Goal goal)
        {
            //only work with the objects that actually have a broken reference
            if (!SerializationUtility.HasManagedReferencesWithMissingTypes(goal))
            {
                return false;
            }
            //clear the error flag
            SerializationUtility.ClearAllManagedReferencesWithMissingTypes(goal);
            EditorUtility.SetDirty(goal);

            return true;
        }
    }
}
#endif