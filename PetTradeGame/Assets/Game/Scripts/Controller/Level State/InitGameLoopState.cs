using System.Collections;

namespace Game.Scripts.Controller.Level_State
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
            if(owner.debugMode) 
                owner.ChangeState<IntroState>();
            else
                owner.ChangeState<MainMenuState>();
        }
    }
}
