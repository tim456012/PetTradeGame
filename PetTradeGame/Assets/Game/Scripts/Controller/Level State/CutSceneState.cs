using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.EventArguments;
using Assets.Game.Scripts.Model;
using UnityEngine;

namespace Assets.Game.Scripts.Controller.Level_State
{
    public class CutSceneState : LevelState
    {
        private ConversationController conversationController;
        private ConversationData conversationData;

        protected override void Awake()
        {
            base.Awake();
            conversationController = owner.GetComponentInChildren<ConversationController>();
            conversationData = Resources.Load<ConversationData>("Conversations/IntroScene");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (conversationData)
            {
                Resources.UnloadAsset(conversationData);
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
            ConversationController.completeEvent += OnCompleteConversation;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            ConversationController.completeEvent -= OnCompleteConversation;
        }

        protected override void OnClick(object sender, InfoEventArgs<Vector3> e)
        {
            base.OnClick(sender, e);
            conversationController.Next();
        }

        private void OnCompleteConversation(object sender, EventArgs e)
        {
            //owner.ChangeState<LevelState>();
        }
    }
}