/*
 * Description: Handles main UI logic.  This is a simple example and should be tailored to your game
 */

using Gameplay.System;
using Service.Framework;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.UI
{    
    public class MainUI : MonoBehaviour
    {
        /*
         * need activeQuest QuestID.name.
         * public method called via button.  button references the questentryui's quest id and passes it
         * when AddObjective is called from quest entry ui, it also calls mainui and passes the data.id and the questid
         * in update active quest it adds to a list and then sets the title text via questid.name
         * it checks to see if any objective is currently complete by iterating over spawnedObjectives and then data[i].iscomplete
         * if true, removeat(i) then continue
         * note:  we need to add some effects here.  an invoked message will help
         * then we set the text of the new objective using data.objectiveText.
         * 
         * public method UpdateActiveQuest(questid, objectivedata)
         * - 
         * 
         * HOW OBJECTIVES ARE ADDED TO THE MENU
         * AddObjective action adds it to the dictionary via goaltrackerdatabase
         * database invokes a message
         * quest log ui hears it
         * it instantiates the quest entry
         * adds it to the active quests list
         * quest log ui then calls addobjective on the newly instantiated quest entry
         * doing so through a for loop.  using the number of objectives in the database, 
         * starting at the current index count of all objectives associated with the quest aka most recently added objective for that quest
         * in quest entry it instantiates an objective for each of those newly added         * 
         * being destroyed or strikethrough via the objectiveentryui depending upon the iscompleted bool
         * 
         * HOW REAL TIME OBJECTIVE UI WORKS
         * set tracked quest is called via a button press on quest entry ui
         * iterates over the number of objectives currently active
         * problem:  this will instantiate a new one every time a new objective is added.  it does not remove one that is completed
         * 
         * 
         */
        [SerializeField]
        private InputActionAsset inputActions;
        protected InputAction MenuButtonAction { get; set; }

        [SerializeField]
        private GameObject menu;

        [SerializeField]
        private GameObject contextualButtonUi;
        public GameObject ContextualButtonUI
        {
            get { return contextualButtonUi; }
            set { contextualButtonUi = value; }
        }

        private UIStateHandler stateHandler;

        [SerializeField]
        private TMP_Text trackedQuestText;
        [SerializeField]
        private TMP_Text currentObjectiveText;

        private QuestID trackedQuest;
        private List<ObjectiveData> objectiveData = new List<ObjectiveData>();

        private GoalTrackerDatabase database;

        [SerializeField]
        private ObjectiveTrackerUI objectivePrefab;

        private List<ObjectiveTrackerUI> currentTrackedObjectives = new List<ObjectiveTrackerUI>();

        private void Awake()
        {
            MenuButtonAction = inputActions.FindAction("Menu");
            MenuButtonAction.started += OnToggleMenu;
        }

        private void Start()
        {
            stateHandler = ReferenceRegistry.Instance.UiStateHandler;
            stateHandler.OnUiStateChanged.AddListener(OnStateChanged);
            database = GoalManager.Instance.GoalTracker;
        }

        public void SetTrackedQuest(QuestID questID)
        {
            //PROBLEM:  can't reselect.  allow this anyway OR require a flag
            if (trackedQuest == questID)
            {
                return;
            }
            //clear old quest list
            trackedQuestText.text = string.Empty;

            trackedQuest = questID;
            List<ObjectiveData> data = database.GetObjectives(questID);
                        
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].IsComplete)
                {
                    continue;
                }
                UpdateActiveQuest(trackedQuest, data[i]);
            }
        }

        public void UpdateActiveQuest(QuestID questID, ObjectiveData data)
        {
            if (trackedQuest != questID)
            {
                return;
            }

            if (database.IsQuestComplete(questID))
            {
                objectiveData.Clear();
                
                //invoke quest complete here

                //clear everything

                return;
            }
            trackedQuestText.text = questID.questName;

            if (data.IsComplete)
            {
                ObjectiveTrackerUI completedObjective = currentTrackedObjectives.Find(d => d.ObjectiveID == data.ID);
                Destroy(completedObjective.gameObject);
                currentTrackedObjectives.Remove(completedObjective);
                data = null;
                return;
            }

            ObjectiveTrackerUI newObjectiveText = Instantiate(objectivePrefab, transform);
            newObjectiveText.GetComponent<TMP_Text>().text = data.ObjectiveText;
            newObjectiveText.Initialize(data);

            currentTrackedObjectives.Add(newObjectiveText);


            //List<ObjectiveData> objectives = database.GetObjectives(questID);
            
            ////iterate over, quests that are not complete, add the objective text
            //for (int i = 0; i < objectives.Count; i++)
            //{
            //    if (objectives[i].IsComplete)
            //    {
            //        objectiveData.RemoveAt(i);
            //        continue;
            //    }

            //    if (objectiveData.Count > 0 && objectiveData[i].ID == objectives[i].ID)
            //    {
            //        continue;
            //    }
            //    objectiveData.Add(objectives[i]);

            //    //invoke objective updated here

            //    currentObjectiveText.text = objectiveData[i].ObjectiveText;
            //}
        }

        protected virtual void OnToggleMenu(InputAction.CallbackContext context)
        {
            //consider this menu part of the "default" ui state layer.
            //this is to prevent other UI logic from interrupting or conflicting
            if (stateHandler.uiState != UIStateHandler.UIState.Default)
            {
                menu.SetActive(false);
                return;
            }
            if (context.started)
            {               
                //set the values based on the opposite state of the menu
                menu.SetActive(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.ToggleCursorLock(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.SetControl(!menu.activeSelf);
            }
        }

        private void OnStateChanged(UIStateHandler.UIState state)
        {
            //set default values when entering a new state
            if (stateHandler.uiState == UIStateHandler.UIState.Default)
            {

            }
            else if (stateHandler.uiState == UIStateHandler.UIState.Gameplay)
            {
                menu.SetActive(false);
            }
            else if (stateHandler.uiState == UIStateHandler.UIState.Pause)
            {
                menu.SetActive(false);
            }
        }

        /// <summary>
        /// Toggles on or off the contextual action button.
        /// </summary>
        /// <param name="state"></param>
        public void SetContextualUiVisible(bool state)
        {
            contextualButtonUi.SetActive(state);
        }

        private void OnDestroy()
        {
            MenuButtonAction.started -= OnToggleMenu;
            stateHandler.OnUiStateChanged.RemoveListener(OnStateChanged);

        }
    }
}