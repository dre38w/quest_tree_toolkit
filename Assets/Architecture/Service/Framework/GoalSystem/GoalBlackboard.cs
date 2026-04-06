/*
 * Description:  Stores unique individual data that can be accessed to perform such actions like 
 *              comparing values, setting values to other data, etc.
 * 
 * NOTE: It is important to note that the keys in each dictionary are strings 
 *      and when they get stored in this database they need to have unique names.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Service.Framework.GoalManagement
{
    [Serializable]
    public class GoalBlackboard
    {
        /// <summary>
        /// The dictionaries that hold the data
        /// </summary>
        private Dictionary<string, bool> goalBoolData = new Dictionary<string, bool>();
        private Dictionary<string, int> goalIntData = new Dictionary<string, int>();
        private Dictionary<string, float> goalFloatData = new Dictionary<string, float>();
        private Dictionary<string, Vector3> goalVector3Data = new Dictionary<string, Vector3>();

        /// <summary>
        /// Get the bool value
        /// </summary>
        /// <param name="name">The key we want the data from</param>
        /// <param name="defaultValue">If no data is stored for this key, return a default</param>
        /// <returns></returns>
        public bool GetBoolValue(string name, bool defaultValue = false)
        {
            if (goalBoolData.TryGetValue(name, out bool value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Set the bool value
        /// </summary>
        /// <param name="name">Pass a key to set</param>
        /// <param name="value">Pass the value to store with that key</param>
        public void SetBoolValue(string name, bool value)
        {
            goalBoolData[name] = value;
        }

        /// <summary>
        /// Get the integer value
        /// </summary>
        /// <param name="name">The key we are wanting the data from</param>
        /// <param name="defaultValue">If that key has no prior data, return a default</param>
        /// <returns></returns>
        public int GetIntValue(string name, int defaultValue = 0)
        {
            if (goalIntData.TryGetValue(name, out int value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Set an integer value
        /// </summary>
        /// <param name="name">The key we want to set</param>
        /// <param name="value">The value we want to set on that key</param>
        public void SetIntValue(string name, int value)
        {
            goalIntData[name] = value;
        }

        /// <summary>
        /// Get the float value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public float GetFloatValue(string name, float defaultValue = 0.0f)
        {
            if (goalFloatData.TryGetValue(name, out float value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a float value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetFloatValue(string name, float value)
        {
            goalFloatData[name] = value;
        }

        /// <summary>
        /// Get a Vector3 value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Vector3 GetVector3Value(string name)
        {
            if (goalVector3Data.TryGetValue(name, out Vector3 value))
            {
                return value;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Set a Vector3 value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetVector3Value(string name, Vector3 value)
        {
            goalVector3Data[name] = value;
        }
    }
}