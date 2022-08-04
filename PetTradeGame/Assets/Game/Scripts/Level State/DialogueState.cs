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
    //TODO: Load Middle Dialogue and show up randomly
    public class DialogueState : GameCore
    {
        private AssetReference _conversationAsset;
        private ConversationController _conversationController;
        private ConversationData _introConversationData, _middleConversationData;

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
            LoadConversationData();
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
            if (_introConversationData)
                Addressables.Release(_introConversationData);

            if (_middleConversationData)
                Addressables.Release(_middleConversationData);

            yield return null;
            _conversationAsset = null;
            _conversationController.enabled = false;
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Changing to main game state");
            Owner.ChangeState<MainGameState>();
        }

        private async void LoadConversationData()
        {
            _conversationAsset = Owner.LevelData.introDialogue;
            _introConversationData = await _conversationAsset.LoadAssetAsync<ConversationData>().Task;

            _conversationAsset = Owner.LevelData.middleDialogue;
            _middleConversationData = await _conversationAsset.LoadAssetAsync<ConversationData>().Task;
            ShowDialogue(_introConversationData);
        }

        private void ShowDialogue(ConversationData dialogue)
        {
            _conversationController.enabled = true;
            _conversationController.Show(dialogue);
        }

        public void OnMiddleDialogue()
        {
            ShowDialogue(_middleConversationData);
        }
    }
}