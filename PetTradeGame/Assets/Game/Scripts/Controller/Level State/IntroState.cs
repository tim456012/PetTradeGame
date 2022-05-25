using System;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class IntroState : GameLoopState
    {
        private ConversationController _conversationController;
        private ConversationData _conversationData;

        protected override void Awake()
        {
            base.Awake();
            _conversationController = Owner.GetComponentInChildren<ConversationController>();
            _conversationData = Owner.levelData.introDialogue;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_conversationData)
            {
                //Resources.UnloadAsset(conversationData);
                _conversationData = null;
            }
        }

        public override void Enter()
        {
            base.Enter();
            _conversationController.Show(_conversationData);
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
            _conversationController.Next();
        }

        private void OnCompleteConversation(object sender, EventArgs e)
        {
            Owner.ChangeState<MainGameState>();
        }
    }
}
