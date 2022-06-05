using System;

namespace Game.Scripts.Controller.Level_State
{
    public class MainMenuState : GameLoopState
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
            _uiController.enabled = true;
            
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
