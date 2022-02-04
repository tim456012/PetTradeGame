using System;
using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Common.State_Machine;
using Assets.Game.Scripts.EventArguments;

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
            InputController.ClickedEvent += OnClick;
        }

        protected override void RemoveListeners()
        {
            InputController.ClickedEvent -= OnClick;
        }

        protected virtual void OnClick(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log("Detected Clicked");
        }
    }
}
