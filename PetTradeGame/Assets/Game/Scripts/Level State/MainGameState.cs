using System;
using System.Collections.Generic;
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

        private bool _isBusy;

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
        }

        public override void Enter()
        {
            base.Enter();
            Init();
            _uiController.ShowGameplayPanel();

            List<RecipeData> recipeDataList = Owner.LevelData.documentRecipeData;
            List<ScoreData> scoreDataList = Owner.LevelData.scoreData;
            List<FunctionalObjectsData> functionObjectDataList = Owner.LevelData.functionalObjectsData;

            _factoryController.InitFactory(recipeDataList, scoreDataList);
            _objectController.InitFunctionalObject(functionObjectDataList);
            Debug.Log("Entering playing state");
        }

        public override void Exit()
        {
            base.Exit();
            _uiController.ShowEndGamePanel();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            GamePlayController.GameFinishEvent += OnGameFinishEvent;
            GamePlayController.OnGamePause += OnGamePauseEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
            GamePlayController.OnGamePause -= OnGamePauseEvent;
        }

        private void Init()
        {
            _objectController.Init();
            _gamePlayController.Init();
            _gamePlayController.SetTimer(Owner.stopTimer);
        }

        private void LoadData()
        {

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
            _isBusy = true;
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
            _isBusy = false;
        }

        private void OnGameFinishEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Over");
            while (_isBusy)
            {
                Debug.Log("System is busy.");
            }
            
            _objectController.StopProcess();
            _factoryController.Release();
            _objectController.Release();
            
            Owner.ChangeState<EndGameState>();
        }
        
        private void OnGamePauseEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Paused");
            _uiController.ShowIpadPanel();
        }

        #endregion
    }
}