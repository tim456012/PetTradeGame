using System;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller.Level_State
{
    public class CutSceneState : GameLoopState
    {
        private CutsceneController cutsceneController;
        private VideoClip video;

        protected override void Awake()
        {
            base.Awake();
            cutsceneController = owner.GetComponentInChildren<CutsceneController>();
            video = owner.LevelData.cutSceneVideo;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            /*if (conversationData)
            {
                //Resources.UnloadAsset(conversationData);
                conversationData = null;
            }*/
        }

        public override void Enter()
        {
            base.Enter();
            cutsceneController.playCutScene(video);
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            CutsceneController.CompleteEvent += OnCompleteVideoPlaying;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            CutsceneController.CompleteEvent -= OnCompleteVideoPlaying;
        }

        private void OnCompleteVideoPlaying(object sender, EventArgs e)
        {
            owner.ChangeState<IntroState>();
        }
    }
}
