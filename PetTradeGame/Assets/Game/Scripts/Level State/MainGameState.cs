using System;
using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Level_State
{
    public class MainGameState : GameCore
    {
        private FactoryController _factoryController;
        private GamePlayController _gamePlayController;
        private ObjectController _objectController;
        private UIController _uiController;
        private ConversationController _conversationController;
        
        private bool _isCompliant, _firstInit = true;
        
        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _objectController = Owner.GetComponentInChildren<ObjectController>();
            _factoryController = Owner.GetComponentInChildren<FactoryController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
            _conversationController = Owner.GetComponentInChildren<ConversationController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayController.GameFinishEvent -= OnLevelFinishEvent;
            GamePlayController.StopProduceDocument -= StopProduceDocument;
            GamePlayController.StartCompliantEvent -= OnStartCompliant;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
            GamePlayPanel.ClearData -= OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc -= OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc -= OnScaleDocumentDownEvent;
            
            ObjectController.SetAnimalGuideEvent -= OnAnimalGuideEvent;
        }

        public override void Enter()
        {
            base.Enter();
            _uiController.ShowGameplayPanel();

            if (_firstInit)
            {
                Init();

                var recipeDataList = Owner.LevelData.documentRecipeData;
                var scoreDataList = Owner.LevelData.scoreData;
                var functionObjectDataList = Owner.LevelData.functionalObjectsData;

                _factoryController.InitFactory(recipeDataList, scoreDataList);
                _objectController.InitFunctionalObject(functionObjectDataList);
                _firstInit = false;
            }
            else if(_isCompliant)
            {
                _gamePlayController.SetTimer(false);
                _isCompliant = false;
                InputController.IsPause = false;
            }
            else
            {
                _gamePlayController.HasCompliant(false);
                _gamePlayController.SetTimer(false);
                _objectController.ReGenerateDocument();
            }
            
            Debug.Log("Entering playing state");
        }

        public override void Exit()
        {
            base.Exit();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            
            GamePlayController.GameFinishEvent += OnLevelFinishEvent;
            GamePlayController.StopProduceDocument += StopProduceDocument;
            GamePlayController.StartCompliantEvent += OnStartCompliant;
            
            GamePlayPanel.ShowIpadView += OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView += OnHideIpadViewEvent;
            GamePlayPanel.GamePause += OnGamePauseEvent;
            GamePlayPanel.GameResume += OnGameResumeEvent;
            GamePlayPanel.ClearData += OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc += OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc += OnScaleDocumentDownEvent;
            
            ObjectController.SetAnimalGuideEvent += OnAnimalGuideEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayController.GameFinishEvent -= OnLevelFinishEvent;
            GamePlayController.StopProduceDocument -= StopProduceDocument;
            GamePlayController.StartCompliantEvent -= OnStartCompliant;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
            GamePlayPanel.ClearData -= OnClearDataEvent;
            GamePlayPanel.ScaleUpDoc -= OnScaleDocumentUpEvent;
            GamePlayPanel.ScaleDownDoc -= OnScaleDocumentDownEvent;
            
            ObjectController.SetAnimalGuideEvent -= OnAnimalGuideEvent;
        }

        private void Init()
        {
            _objectController.Init();
            _gamePlayController.Init();
            _gamePlayController.SetTimer(Owner.stopTimer);
            _uiController.DisableIpadButton(false);
            _objectController.InitAnimalGuide(Owner.LevelData.animalGuide);
        }

        private void LoadData()
        {

        }

        private IEnumerator Release()
        {
            yield return null;
            
            _factoryController.ReleaseDocument();
            _factoryController.Release();
            _objectController.Release();
            _conversationController.Release();
            //yield return new WaitForSeconds(2f);
            yield return null;
            
            Owner.ChangeState<EndGameState>();
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
            var id = _factoryController.generatedID;
            var content = Owner.LevelData.scoreData;
            foreach (ScoreData scoreContent in content)
            {
                if (!id.Equals(scoreContent.id))
                    continue;
                _gamePlayController.CalculateScore(e.info, scoreContent.score, scoreContent.isWrongDocument);
            }
            _factoryController.ReleaseDocument();
            
            _gamePlayController.SetTimer(true);
            _uiController.HideGameplayPanel();
            _conversationController.StartConversation();
            Owner.ChangeState<DialogueState>();
            
            _objectController.ReGenerateLicense();
        }

        private void OnLevelFinishEvent(object sender, EventArgs e)
        {
            _firstInit = true;
            Debug.Log("Game Over");
            
            InputController.IsDragActive = false;
            _uiController.ShowEndGamePanel();
            StartCoroutine(Release());
        }

        private void StopProduceDocument(object sender, EventArgs e)
        {
            Debug.Log("Stop producing document.");
            _uiController.DisableIpadButton(true);
            _objectController.StopProcess();
        }
        
        private void OnShowIpadViewEvent(object sender, EventArgs e)
        {
            _gamePlayController.SetTimer(true);
            _uiController.ShowIpadViewPanel();
        }
        
        private void OnHideIpadViewEvent(object sender, EventArgs e)
        {
            _gamePlayController.SetTimer(false);
            _uiController.HideIpadViewPanel();
        }
        
        private void OnGamePauseEvent(object sender, EventArgs e)
        {
            _gamePlayController.SetTimer(true);
            _uiController.OnBtnSettingClicked();
        }
        
        private void OnGameResumeEvent(object sender, EventArgs e)
        {
            _gamePlayController.SetTimer(false);
            _uiController.OnBtnResumeClicked();
        }
        
        private void OnAnimalGuideEvent(object sender, InfoEventArgs<Sprite> e)
        {
            _uiController.SetAnimalGuide(e.info);
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

        private void OnStartCompliant(object sender, EventArgs e)
        {
            InputController.IsPause = true;
            InputController.IsDragActive = false;
            _isCompliant = true;
            _uiController.HideGameplayPanel();
            Owner.ChangeState<DialogueState>();
            _conversationController.StartConversation(true);
        }

        #endregion
    }
}