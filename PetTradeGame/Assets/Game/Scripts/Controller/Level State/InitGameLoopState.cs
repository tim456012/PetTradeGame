using System.Collections;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class InitGameLoopState : GameLoopState
    {
        private SpriteRenderer _spriteRenderer;
        
        protected override void Awake()
        {
            base.Awake();
            ScaleGameWorld();
        }

        public override void Enter()
        {
            base.Enter();
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return null;
            if(Owner.debugMode) 
                Owner.ChangeState<IntroState>();
            else
                Owner.ChangeState<MainMenuState>();
        }

        private void ScaleGameWorld()
        {
            GameObject background = GameObjFinder.FindChildGameObject(Owner.world.gameObject, "Background");
            _spriteRenderer = background.GetComponent<SpriteRenderer>();
            
            float worldScreenHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            background.transform.localScale = new Vector3(
                worldScreenWidth / _spriteRenderer.sprite.bounds.size.x,
                worldScreenHeight / _spriteRenderer.sprite.bounds.size.y, 1);
        }
    }
}
