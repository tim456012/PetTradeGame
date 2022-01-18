using System;
using Game.Scripts.EventArgs;
using UnityEngine;

namespace Assets.Game.Scripts.Controller
{
    public class InputController : MonoBehaviour
    {
        //public static event EventHandler<InfoEventArgs<GameObject>> obj;
        public static event EventHandler<InfoEventArgs<int>> clickEvent;

        private string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" };

        private void Update()
        {
            for (int i = 0; i < 3; ++i)
            {
                if (Input.GetButtonUp(_buttons[i]))
                {
                    clickEvent?.Invoke(this, new InfoEventArgs<int>(i));
                }
            }
        }
    }
}
