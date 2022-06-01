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
            _uiController.SetScore(_score, 0);
        }
    }
}
