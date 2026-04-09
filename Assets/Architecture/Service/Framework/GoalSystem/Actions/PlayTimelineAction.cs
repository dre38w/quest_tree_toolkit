/*
 * Description: Plays a timeline.  Useful for triggering cut scenes, playing animations on objects, etc.
 */
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Service.Framework.Goals
{
    public class PlayTimelineAction : ObjectiveAction
    {
        [SerializeField]
        private PlayableDirector timelineDirector;

        [SerializeField]
        private TimelineAsset timeline;

        private void Start()
        {
            timelineDirector.playableAsset = timeline;
        }

        public override void InitializeAction()
        {
            timelineDirector.Play(timeline);
            timelineDirector.stopped += OnDirectorComplete;
        }

        private void OnDirectorComplete(PlayableDirector playableDirector)
        {
            //complete after the timeline finished playing
            SetComplete();
            timelineDirector.stopped -= OnDirectorComplete;
        }
    }
}