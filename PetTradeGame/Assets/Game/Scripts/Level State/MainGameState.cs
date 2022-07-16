﻿using System;
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
            _factoryController.Release();
            _objectController.Release();


            _uiController.ShowEndGamePanel();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            GamePlayController.GameFinishEvent += OnGameFinishEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
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
            string id = _objectController.GetGeneratedID();
            List<ScoreData> content = Owner.LevelData.scoreData;
            foreach (ScoreData scoreContent in content)
            {
                if (!id.Equals(scoreContent.id))
                    continue;
                _gamePlayController.CalculateScore(e.info, scoreContent.score, scoreContent.isWrongDocument);
                _factoryController.Release();
            }
            _objectController.ReGenerateLicense();
            _objectController.ReGenerateDocument();
        }

        private void OnGameFinishEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Over");
            _objectController.StopProcess();
            Owner.ChangeState<EndGameState>();
        }

        #endregion
    }
}