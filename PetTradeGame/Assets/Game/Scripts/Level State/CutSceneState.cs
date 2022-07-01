using System;
using Game.Scripts.Controller;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Level_State
{
    public class CutSceneState : GameCoreState
    {
        private CutsceneController _cutsceneController;
        private VideoClip _video1, _video2, _video3;
        private bool _hasVideo2, _hasVideo3;

        protected override void Awake()
        {
            base.Awake();
            _cutsceneController = Owner.GetComponentInChildren<CutsceneController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!_video1)
                return;

            Resources.UnloadAsset(_video1);
            _video1 = null;
            if (!_video2)
                return;
            
            Resources.UnloadAsset(_video2);
            _video2 = null;
            if(!_video3)
                return;
            
            Resources.UnloadAsset(_video3);
            _video3 = null;
        }

        public override void Enter()
        {
            base.Enter();
            _video1 = Owner.levelData.video1;
            _video2 = Owner.levelData.video2;
            _video3 = Owner.levelData.video3;

            if (_video2 != null)
                _hasVideo2 = true;

            if (_video3 != null)
                _hasVideo3 = true;
            
            _cutsceneController.PlayCutScene(_video1);
            //CheckVideoToPlay();
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

        private void CheckVideoToPlay()
        {
            if (!_hasVideo2 && !_hasVideo3)
                return;
           
            _cutsceneController.PlayCutScene(_video2);
            _hasVideo2 = false;

            //if (!_hasVideo3)
            //    return;
            
            //_cutsceneController.PlayCutScene(_video3);
            //_hasVideo3 = false;
        }

        private void OnCompleteVideoPlaying(object sender, EventArgs e)
        {
            //CheckVideoToPlay();
            if(!_hasVideo2 && !_hasVideo3)
              Owner.ChangeState<IntroState>();
        }
    }
}
