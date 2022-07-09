using Game.Scripts.Common.State_Machine;
using Game.Scripts.Controller;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Level_State
{
    public abstract class GameCore : State
    {
        protected GameManager Owner;

        public LevelData LevelData => Owner.LevelData;

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
