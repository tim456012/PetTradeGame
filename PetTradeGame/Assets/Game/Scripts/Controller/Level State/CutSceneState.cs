using System;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class CutSceneState : GameLoopState
    {
        private CutsceneController cutsceneController;
        //private ConversationData conversationData;

        protected override void Awake()
        {
            base.Awake();
            cutsceneController = owner.GetComponentInChildren<CutsceneController>();
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
            cutsceneController.playCutScene();
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