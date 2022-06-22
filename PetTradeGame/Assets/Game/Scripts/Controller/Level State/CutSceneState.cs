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
        private VideoClip _introVideo, _dayIntroVideo;
        private bool _isNextVideoPlay;

        protected override void Awake()
        {
            base.Awake();
            _cutsceneController = Owner.GetComponentInChildren<CutsceneController>();

            if (_dayIntroVideo != null)
                _isNextVideoPlay = true;
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
            _introVideo = Owner.levelData.introVideo;
            _dayIntroVideo = Owner.levelData.dayIntroVideo;

            _cutsceneController.PlayCutScene(_introVideo);
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
            if (_isNextVideoPlay)
            {
                _cutsceneController.PlayCutScene(_dayIntroVideo);
                _isNextVideoPlay = false;
                return;
            }
            
            Owner.ChangeState<IntroState>();
        }
    }
}
