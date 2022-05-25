using System;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller.Level_State
{
    public class CutSceneState : GameLoopState
    {
        private CutsceneController _cutsceneController;
        private VideoClip _video;

        protected override void Awake()
        {
            base.Awake();
            _cutsceneController = Owner.GetComponentInChildren<CutsceneController>();
            _video = Owner.levelData.firstVideo;
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
            _cutsceneController.playCutScene(_video);
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
            Owner.ChangeState<IntroState>();
        }
    }
}
