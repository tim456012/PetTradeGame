using System;
using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Common.State_Machine;
using Assets.Game.Scripts.Controller.Level_State;

namespace Assets.Game.Scripts.Controller
{
    public class LevelController : StateMachine
    {
        //public LevelData LevelData;

        private void Start()
        {
            ChangeState<InitLevelState>();
        }
    }
}
