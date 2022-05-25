using Game.Scripts.Common.State_Machine;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public abstract class GameLoopState : State
    {
        protected GameManager Owner;

        public LevelData LevelData => Owner.levelData;

        protected virtual void Awake()
        {
            Owner = GetComponent<GameManager>();
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
