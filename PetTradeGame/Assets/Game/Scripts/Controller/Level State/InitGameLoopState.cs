using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Controller.Level_State
{
    public class InitGameLoopState : GameLoopState
    {
        public override void Enter()
        {
            base.Enter();
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return null;
            owner.ChangeState<CutSceneState>();
        }
    }
}