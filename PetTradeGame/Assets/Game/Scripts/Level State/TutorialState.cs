using System;
using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.EventArguments;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Level_State
{
    public class TutorialState : GameCore
    {
        private FactoryController _factoryController;
        private GamePlayController _gamePlayController;
        private ObjectController _objectController;
        private UIController _uiController;
        private ConversationController _conversationController;

        private bool _hasShowIpad, _hasShowAnimal, _hasSubmitted, _firstInit, _isBackToMenu;

        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _objectController = Owner.GetComponentInChildren<ObjectController>();
            _factoryController = Owner.GetComponentInChildren<FactoryController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
            _conversationController = Owner.GetComponentInChildren<ConversationController>();
            _firstInit = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
            GamePlayPanel.ClearData -= OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc -= OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc -= OnScaleDocumentDownEvent;
            GamePlayPanel.TutorialIpadEvent -= OnTutorialIpadClicked;
            GamePlayPanel.HideTutorialIpadEvent -= OnTutorialIpadHide;
            GamePlayPanel.BackToMenuEvent -= OnBackToMenuEvent;

            ObjectController.SetAnimalGuideEvent -= OnAnimalGuideEvent;
        }

        public override void Enter()
        {
            base.Enter();
            _uiController.ShowGameplayPanel();
            if (_firstInit)
            {
                InputController.IsDragActive = false;
                _firstInit = false;
                StartCoroutine(Init());
            }
            else if (!_hasShowIpad)
            {
                var recipeDataList = Owner.LevelData.documentRecipeData;
                var scoreDataList = Owner.LevelData.scoreData;
                var functionObjectDataList = Owner.LevelData.functionalObjectsData;

                _factoryController.InitFactory(recipeDataList, scoreDataList);
                StartCoroutine(_objectController.InitFunctionalObject(functionObjectDataList));
            }
            else if (_hasShowIpad && _hasShowAnimal && _hasSubmitted)
            {
                _objectController.StopProcess();

                InputController.IsDragActive = false;
                StartCoroutine(Release());
            }

            //InputController.IsPause = false;
            Debug.Log("Entering playing state");
        }

        public override void Exit()
        {
            base.Exit();
            Owner.world.SetActive(false);
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            
            GamePlayPanel.ShowIpadView += OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView += OnHideIpadViewEvent;
            GamePlayPanel.GamePause += OnGamePauseEvent;
            GamePlayPanel.GameResume += OnGameResumeEvent;
            GamePlayPanel.ClearData += OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc += OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc += OnScaleDocumentDownEvent;
            GamePlayPanel.TutorialIpadEvent += OnTutorialIpadClicked;
            GamePlayPanel.HideTutorialIpadEvent += OnTutorialIpadHide;
            GamePlayPanel.BackToMenuEvent += OnBackToMenuEvent;
            
            ObjectController.SetAnimalGuideEvent += OnAnimalGuideEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
            GamePlayPanel.ClearData -= OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc -= OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc -= OnScaleDocumentDownEvent;
            GamePlayPanel.TutorialIpadEvent -= OnTutorialIpadClicked;
            GamePlayPanel.HideTutorialIpadEvent -= OnTutorialIpadHide;
            GamePlayPanel.BackToMenuEvent -= OnBackToMenuEvent;

            ObjectController.SetAnimalGuideEvent -= OnAnimalGuideEvent;
        }

        private IEnumerator Init()
        {
            _objectController.Init();
            _gamePlayController.Init();
            _gamePlayController.SetTimer(true);
            _uiController.DisableIpadButton(false);
            _objectController.InitAnimalGuide(Owner.LevelData.animalGuide);
            yield return null;
            
            //TODO: transition
            Debug.Log("Doing transition");
            
            LoadConversation(1);
        }
        
        private IEnumerator Release()
        {
            _hasShowIpad = false;
            _hasShowAnimal = false;
            _hasSubmitted = false;
            yield return null;

            _factoryController.ReleaseDocument();
            _factoryController.Release();
            _objectController.Release();
            _conversationController.Release();
            yield return null;

            Owner.isTutorial = false;
            _gamePlayController.Reset();
            
            Owner.ChangeState<MainMenuState>();
        }

        private void LoadConversation(int index)
        {
            _uiController.HideGameplayPanel();
            _conversationController.StartTutorialConversation(index);
            Owner.ChangeState<DialogueState>();
        }
        
        #region Event Behaviors

        private void OnObjCollision(object sender, InfoEventArgs<GameObject> col)
        {
            var original = sender as GameObject;
            if (original == null)
                return;

            _objectController.ProcessCollision(original, col.info);
        }

        private void OnSubmitted(object sender, InfoEventArgs<int> e)
        {
            if(!_hasShowIpad && !_hasShowAnimal && !_hasSubmitted)
            {
                StartCoroutine(_objectController.ReGenerateLicense());
                return;
            }

            _factoryController.ReleaseDocument();
            _hasSubmitted = true;
            LoadConversation(5);
        }
        
        private void OnShowIpadViewEvent(object sender, EventArgs e)
        {
            InputController.IsDragActive = false;
            _uiController.ShowIpadViewPanel();
        }

        private void OnHideIpadViewEvent(object sender, EventArgs e)
        {
            InputController.IsDragActive = false;
            //InputController.IsPause = false;
            _uiController.HideIpadViewPanel();
        }

        private void OnGamePauseEvent(object sender, EventArgs e)
        {
            _uiController.OnBtnSettingClicked();
        }

        private void OnGameResumeEvent(object sender, EventArgs e)
        {
            _uiController.OnBtnResumeClicked();
        }

        private void OnAnimalGuideEvent(object sender, InfoEventArgs<Sprite> e)
        {
            _uiController.SetAnimalGuide(e.info);
            if(_hasShowAnimal)
                return;
            
            _hasShowAnimal = true;
            _uiController.DisableIpadButton(false);
            LoadConversation(3);
        }

        private void OnClearDataEvent(object sender, EventArgs e)
        {
            Owner.ClearGameData();
            _uiController.OnBtnSettingClicked();
        }

        private void OnScaleDocumentUpEvent(object sender, EventArgs e)
        {
            _objectController.ScaleDocument();
        }

        private void OnScaleDocumentDownEvent(object sender, EventArgs e)
        {
            _objectController.ScaleDocument();
        }
        
        private void OnTutorialIpadClicked(object sender, EventArgs e)
        {
            if(_hasShowIpad)
                return;

            _uiController.DisableIpadButton(true);
            _hasShowIpad = true;
            LoadConversation(2);
        }

        private void OnTutorialIpadHide(object sender, EventArgs e)
        {
            LoadConversation(4);
        }
        
        private void OnBackToMenuEvent(object sender, EventArgs e)
        {
            _firstInit = true;
            DialogueState.FirstInit = true;
            _objectController.StopProcess();
            InputController.IsDragActive = false;
            StartCoroutine(Release());
        }

        #endregion
    }
}