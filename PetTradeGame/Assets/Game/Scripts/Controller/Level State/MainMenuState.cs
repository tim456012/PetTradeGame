using System;

namespace Game.Scripts.Controller.Level_State
{
    public class MainMenuState : GameLoopState
    {
        private UIController _uiController;

        protected override void Awake()
        {
            base.Awake();
            _uiController = owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
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
            owner.ChangeState<CutSceneState>();
        }
    }
}
