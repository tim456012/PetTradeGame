using System;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class EndGameState : GameLoopState
    {
        private GamePlayController _gamePlayController;
        private UIController _uiController;

        private int _score;

        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            _score = _gamePlayController.GetScore();
            _uiController.SetScore(_score, _gamePlayController.GetCorrectDoc(), _gamePlayController.GetWrongDoc());
        }

        public override void Exit()
        {
            base.Exit();
            _gamePlayController.Reset();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            UIController.NextDayEvent += OnNextDayEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            UIController.NextDayEvent -= OnNextDayEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIController.NextDayEvent -= OnNextDayEvent;
        }
        
        private void OnNextDayEvent(object sender, EventArgs e)
        {
            Owner.levelData = null;
            GC.Collect();
            string dataName = $"LevelData_Day{++Owner.LevelCount}";
            Debug.Log($"Loading next day data : {dataName}");
            Owner.levelData = Resources.Load<LevelData>($"Level Data/{dataName}");
            Owner.ChangeState<CutSceneState>();
        }
    }
}