using System;
using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Common.State_Machine;

namespace Assets.Game.Scripts.Controller.Level_State
{
    public class LevelState : State
    {
        protected LevelController owner;

        //public LevelData LevelData => owner.LevelData;

        protected virtual void Awake()
        {
            owner = GetComponent<LevelController>();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
        }
        
        
    }
}
