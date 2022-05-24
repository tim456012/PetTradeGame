using Game.Scripts.Common.State_Machine;
using Game.Scripts.Controller.Level_State;
using Game.Scripts.Model;

namespace Game.Scripts.Controller
{
    public class GameManager : StateMachine
    {
        public LevelData levelData;
        public bool debugMode;
        
        private void Start()
        {
            ChangeState<InitGameLoopState>();
        }
    }
}
