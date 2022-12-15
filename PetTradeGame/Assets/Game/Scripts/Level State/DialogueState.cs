using System;
using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Level_State
{
    public class DialogueState : GameCore
    {
        public static bool FirstInit = true;
        
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
            InputController.IsPause = true;
            Owner.world.SetActive(true);
            
            if (!FirstInit) 
                return;

            if (Owner.isTutorial)
            {
                _conversationController.LoadTutorialDialogue(Owner.tutorialDialogue.speakerList);
                FirstInit = false;
                _conversationController.StartTutorialConversation();
                return;
            }
            
            _conversationController.LoadGlobalDialogue(Owner.globalDialogue.speakerList);
            LoadConversationData();
            FirstInit = false;
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
            if(FirstInit)
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

            if (Owner.isTutorial)
            {
                Debug.Log("Changing to tutorial state");
                Owner.ChangeState<TutorialState>();
            }
            else
            {
                Debug.Log("Changing to main game state");
                Owner.ChangeState<MainGameState>();
            }
        }

        private async void LoadConversationData()
        {
            _conversationAsset = Owner.LevelData.dialogue;
            if(_conversationData == null)
                _conversationData = await _conversationAsset.LoadAssetAsync<ConversationData>().Task;
            
            _conversationController.LoadLevelDialogue(_conversationData.speakerList);
            _conversationController.StartConversation();
        }

        private void OnClearConversation(object sender, EventArgs e)
        {
            FirstInit = true;
            if (_conversationData)
                Addressables.Release(_conversationData);
            
            GamePlayController.ClearConversationEvent -= OnClearConversation;
        }
    }
}