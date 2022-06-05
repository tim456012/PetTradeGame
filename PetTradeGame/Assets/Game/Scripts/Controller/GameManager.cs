using Game.Scripts.Common.State_Machine;
using Game.Scripts.Controller.Level_State;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GameManager : StateMachine
    {
        public LevelData levelData;
        public GameObject world;
        public bool debugMode;
        public bool stopTimer;
        
        private void Start()
        {
            ChangeState<InitGameLoopState>();
        }
    }
}
