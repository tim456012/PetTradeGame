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
        
        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _objectController = Owner.GetComponentInChildren<ObjectController>();
            _factoryController = Owner.GetComponentInChildren<FactoryController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
            GamePlayController.StopProduceDocument -= StopProduceDocument;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
        }

        public override void Enter()
        {
            base.Enter();
            Init();
            _uiController.ShowGameplayPanel();

            var recipeDataList = Owner.LevelData.documentRecipeData;
            var scoreDataList = Owner.LevelData.scoreData;
            var functionObjectDataList = Owner.LevelData.functionalObjectsData;

            _factoryController.InitFactory(recipeDataList, scoreDataList);
            _objectController.InitFunctionalObject(functionObjectDataList);
            Debug.Log("Entering playing state");
        }

        public override void Exit()
        {
            base.Exit();

            _objectController.Release();
            _uiController.ShowEndGamePanel();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            
            GamePlayController.GameFinishEvent += OnGameFinishEvent;
            GamePlayController.StopProduceDocument += StopProduceDocument;

            GamePlayPanel.ShowIpadView += OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView += OnHideIpadViewEvent;
            GamePlayPanel.GamePause += OnGamePauseEvent;
            GamePlayPanel.GameResume += OnGameResumeEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
            GamePlayController.StopProduceDocument -= StopProduceDocument;
            
            GamePlayPanel.ShowIpadView -= OnShowIpadViewEvent;
            GamePlayPanel.HideIpadView -= OnHideIpadViewEvent;
            GamePlayPanel.GamePause -= OnGamePauseEvent;
            GamePlayPanel.GameResume -= OnGameResumeEvent;
        }

        private void Init()
        {
            _objectController.Init();
            _gamePlayController.Init();
            _gamePlayController.SetTimer(Owner.stopTimer);
            _uiController.DisableIpadButton(false);
        }

        private void LoadData()
        {

        }

        private IEnumerator Release()
        {
            yield return null;
            
            _factoryController.Release();
            _objectController.Release();
            yield return new WaitForSeconds(2f);
            
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
            _factoryController.Release();
            
            _objectController.ReGenerateLicense();
            _objectController.ReGenerateDocument();
        }

        private void OnGameFinishEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Over");
            InputController.IsDragActive = false;
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
            Debug.Log("Game Paused");
        }
        
        private void OnGameResumeEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Resumed");
        }

        #endregion
    }
}