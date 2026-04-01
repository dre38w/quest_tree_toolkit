
/*
Description: Holds references to the available Goal classes
Handles running various logic for optimization such as Update methods, etc.
*/

using UnityEngine;
using System.Collections.Generic;

namespace Service.Framework.Goals
{
    public class GoalManager : MonoBehaviour
    {
        public static GoalManager Instance;

        private List<Goal> goals = new List<Goal>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            //handle running Update methods for Goal class 
            for (int goalIndex = 0; goalIndex < goals.Count; goalIndex++)
            {
                goals[goalIndex].GoalUpdate(Time.deltaTime);
            }
        }

        public static void AddGoal(Goal goal)
        {
            Instance.goals.Add(goal);
        }

        /// <summary>
        /// When needing to reference a specific goal by its ID
        /// </summary>
        /// <param name="id">The unique ID assigned to each goal</param>
        /// <returns></returns>
        public Goal GetGoal(GoalID id)
        {
            for (int idIndex = 0; idIndex < goals.Count; idIndex++)
            {
                if (id == goals[idIndex].id)
                {
                    return goals[idIndex];
                }
            }
            return null;
        }
    }
}