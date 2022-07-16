using System;
using Game.Scripts.Controller;
using UnityEngine;

namespace Game.Scripts.Level_State
{
    public class EndGameState : GameCore
    {
        private GamePlayController _gamePlayController;

        private int _score;
        private UIController _uiController;

        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIController.NextDayEvent -= OnNextDayEvent;
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

        private void OnNextDayEvent(object sender, EventArgs e)
        {
            GC.Collect();
            string dataName = $"Day{++Owner.LevelCount}";
            Debug.Log($"Loading next day data : {dataName}");
            Owner.ChangeLevel(dataName);
        }
    }
}