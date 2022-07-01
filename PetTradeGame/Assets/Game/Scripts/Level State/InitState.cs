using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.Level_State
{
    public class InitState : GameCoreState
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
            {
                GamePlayController.IsDebugMode = true;
                UIController.IsDebugMode = true;
                Owner.ChangeState<IntroState>();
            }
            else
                Owner.ChangeState<MainMenuState>();
        }

        private void ScaleGameWorld()
        {
            var background = GameObjFinder.FindChildGameObject(Owner.world.gameObject, "Background");
            _spriteRenderer = background.GetComponent<SpriteRenderer>();
            
            float worldScreenHeight = Camera.main!.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            var sprite = _spriteRenderer.sprite;
            background.transform.localScale = new Vector3(
                worldScreenWidth / sprite.bounds.size.x,
                worldScreenHeight / sprite.bounds.size.y, 1);
        }
    }
}
