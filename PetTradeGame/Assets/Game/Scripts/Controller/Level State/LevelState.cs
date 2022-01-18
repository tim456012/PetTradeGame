using System;
using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Common.State_Machine;
using Game.Scripts.EventArgs;

namespace Assets.Game.Scripts.Controller.Level_State
{
    public abstract class LevelState : State
    {
        protected LevelController owner;

        //public LevelData levelData => owner.LevelData;

        protected virtual void Awake()
        {
            owner = GetComponent<LevelController>();
        }

        protected override void AddListeners()
        {
            InputController.clickEvent += OnClick;
        }

        protected override void RemoveListeners()
        {
            InputController.clickEvent -= OnClick;
        }

        protected virtual void OnClick(object sender, InfoEventArgs<int> e)
        {
            
        }
        
        //Select GameObject
    }
}
