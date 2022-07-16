using System;
using Game.Scripts.Controller;

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
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            UIController.StartGameEvent -= OnStartGameEvent;
        }

        private void OnStartGameEvent(object sender, EventArgs e)
        {
            Owner.ChangeState<CutSceneState>();
        }
    }
}