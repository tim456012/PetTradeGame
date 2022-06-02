using System;
using System.Collections.Generic;
using Game.Scripts.Controller.SubController;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class MainGameState : GameLoopState
    {
        private GamePlayController _gamePlayController;
        private ObjectController _objectController;
        private UIController _uiController;
        
        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _objectController = Owner.GetComponentInChildren<ObjectController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            if(Owner.debugMode)
                _uiController.SetDebugMode();
            
            _gamePlayController.enabled = true;
            
            _objectController.gameObject.AddComponent<DragAndDropSubController>();
            _objectController.enabled = true;

            var recipeDataList = Owner.levelData.documentRecipeData;
            var scoreData = Owner.levelData.scoreData;
            
            _objectController.InitFactory(recipeDataList, scoreData);
            _objectController.InitObjectPool(Owner.levelData.functionalObjectsData);

            _gamePlayController.StartTimer();
            Debug.Log("Entering playing state");
        }

        public override void Exit()
        {
            base.Exit();
            _objectController.ReleaseDocuments();
            _objectController.ReleaseInstances();
            _objectController.Release();
            
            _uiController.EndGame();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
            GamePlayController.TimerEvent += OnTimerUpdated;
            GamePlayController.GameFinishEvent += OnGameFinishEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            GamePlayController.TimerEvent -= OnTimerUpdated;
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityAttribute.FunctionalObjCollisionEvent -= OnObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
            GamePlayController.TimerEvent -= OnTimerUpdated;
            GamePlayController.GameFinishEvent -= OnGameFinishEvent;
        }

        private void OnObjCollision(object sender, InfoEventArgs<GameObject> col)
        {
            var original = sender as GameObject;
            if (original == null)
                return;

            _objectController.ProcessCollision(original, col.info);
        }

        private void OnSubmitted(object sender, InfoEventArgs<sbyte> e)
        {
            string id = _objectController.GetGeneratedID();
            var content = Owner.levelData.scoreData.scoreContents;
            foreach (var scoreContent in content)
            {
                if (!id.Equals(scoreContent.id))
                    continue;
                int score = _gamePlayController.CalculateScore(e.info, scoreContent.score);
                _uiController.SetScore(score);
                _objectController.ReleaseDocuments();
            }
            _objectController.ReGenerateDocument();
        }

        private void OnTimerUpdated(object sender, InfoEventArgs<string> e)
        {
            _uiController.SetTime(e.info);
        }

        private void OnGameFinishEvent(object sender, EventArgs e)
        {
            Debug.Log("Game Over");
            _objectController.StopProcess();
            Owner.ChangeState<EndGameState>();
        }
    }
}
