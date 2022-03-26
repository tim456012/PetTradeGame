using System;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class IntroState : GameLoopState
    {
        private ConversationController conversationController;
        private ConversationData conversationData;

        protected override void Awake()
        {
            base.Awake();
            conversationController = owner.GetComponentInChildren<ConversationController>();
            conversationData = owner.LevelData.IntroDialogue;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (conversationData)
            {
                //Resources.UnloadAsset(conversationData);
                conversationData = null;
            }
        }

        public override void Enter()
        {
            base.Enter();
            conversationController.Show(conversationData);
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            ConversationController.CompleteEvent += OnCompleteConversation;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            ConversationController.CompleteEvent -= OnCompleteConversation;
        }

        protected override void OnClick(object sender, InfoEventArgs<Vector3> e)
        {
            base.OnClick(sender, e);
            conversationController.Next();
        }

        private void OnCompleteConversation(object sender, EventArgs e)
        {
            owner.ChangeState<PlayingState>();
        }
    }
}