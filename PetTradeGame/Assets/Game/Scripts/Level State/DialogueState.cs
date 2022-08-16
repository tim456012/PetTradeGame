using System;
using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Level_State
{
    //TODO: Complete Level End event
    public class DialogueState : GameCore
    {
        private bool _firstInit = true;
        
        private AssetReference _conversationAsset;
        private ConversationController _conversationController;
        private ConversationData _conversationData;
        
        protected override void Awake()
        {
            base.Awake();
            _conversationController = Owner.GetComponentInChildren<ConversationController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _conversationAsset = null;
            _conversationController.enabled = false;
        }
        
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Enter dialogue state");

            if (_firstInit)
            {
                _conversationController.LoadGlobalDialogue(Owner.globalDialogue.speakerList);
                LoadConversationData();
                _firstInit = false;
            }
            else
            {
                _conversationController.StartConversation();
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit dialogue state");
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            ConversationController.CompleteEvent += OnCompleteConversation;
            if(_firstInit)
                GamePlayController.ClearConversationEvent += OnClearConversation;
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
            StartCoroutine(ChangeState());
        }

        private IEnumerator ChangeState()
        {
            yield return null;
            _conversationAsset = null;
            _conversationController.enabled = false;
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Changing to main game state");
            Owner.ChangeState<MainGameState>();
        }

        private async void LoadConversationData()
        {
            _conversationAsset = Owner.LevelData.dialogue;
            _conversationData = await _conversationAsset.LoadAssetAsync<ConversationData>().Task;
            
            _conversationController.LoadLevelDialogue(_conversationData.speakerList);
            _conversationController.StartConversation();
        }

        private void OnClearConversation(object sender, EventArgs e)
        {
            _firstInit = true;
            if (_conversationData)
                Addressables.Release(_conversationData);
            
            GamePlayController.ClearConversationEvent -= OnClearConversation;
        }
    }
}