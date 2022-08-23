using System;
using Game.Scripts.Controller;
using Game.Scripts.View_Model_Components;

namespace Game.Scripts.Level_State
{
    public class MainMenuState : GameCore
    {
        private UIController _uiController;

        protected override void Awake()
        {
            base.Awake();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            _uiController.Init();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            UIController.StartGameEvent += OnStartGameEvent;
            UIController.StartTutorialEvent += OnStartTutorialEvent;

            GamePlayPanel.GamePause += OnSettingOpenEvent;
            GamePlayPanel.GameResume += OnSettingCloseEvent;
            GamePlayPanel.ClearData += OnClearDataEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            UIController.StartGameEvent -= OnStartGameEvent;
            UIController.StartTutorialEvent -= OnStartTutorialEvent;

            GamePlayPanel.GamePause -= OnSettingOpenEvent;
            GamePlayPanel.GameResume -= OnSettingCloseEvent;
            GamePlayPanel.ClearData -= OnClearDataEvent;
        }

        private void OnStartGameEvent(object sender, EventArgs e)
        {
            Owner.ChangeState<CutSceneState>();
        }
        
        private void OnStartTutorialEvent(object sender, EventArgs e)
        {
            Owner.isTutorial = true;
            Owner.ChangeState<DialogueState>();
        }

        private void OnSettingOpenEvent(object sender, EventArgs e)
        {
            _uiController.OnBtnSettingClicked();
        }
        
        private void OnSettingCloseEvent(object sender, EventArgs e)
        {
            _uiController.OnBtnResumeClicked();
        }
        
        private void OnClearDataEvent(object sender, EventArgs e)
        {
            Owner.ClearGameData();
            _uiController.OnBtnSettingClicked();
        }
    }
}