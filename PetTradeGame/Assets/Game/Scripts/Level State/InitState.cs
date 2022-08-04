using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Level_State
{
    /// <summary>
    ///     Init every controllers in the game.
    /// </summary>
    public class InitState : GameCore
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

        //Init as debug mode or general mode.
        private IEnumerator Init()
        {
            yield return null;
            if (Owner.debugMode)
            {
                GamePlayController.IsDebugMode = true;
                UIController.IsDebugMode = true;
                Owner.ChangeState<DialogueState>();
            }
            else
            {
                Owner.ChangeState<MainMenuState>();
            }
        }


        private void ScaleGameWorld()
        {
            GameObject background = GameObjFinder.FindChildGameObject(Owner.world.gameObject, "Background");
            _spriteRenderer = background.GetComponent<SpriteRenderer>();

            float worldScreenHeight = Camera.main!.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            Sprite sprite = _spriteRenderer.sprite;
            background.transform.localScale = new Vector3(
                worldScreenWidth / sprite.bounds.size.x,
                worldScreenHeight / sprite.bounds.size.y, 1);
        }
    }
}