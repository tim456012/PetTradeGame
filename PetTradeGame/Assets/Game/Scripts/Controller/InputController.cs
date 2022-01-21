using System;
using Assets.Game.Scripts.EventArguments;
using UnityEngine;

namespace Assets.Game.Scripts.Controller
{
    public class InputController : MonoBehaviour
    {
        public static bool IsDragActive = false;

        public static event EventHandler<InfoEventArgs<Vector3>> ClickedEvent;
        public static event EventHandler<InfoEventArgs<Vector3>> DraggingEvent;
        public static event EventHandler<InfoEventArgs<Vector3>> DroppingEvent;

        private bool _isHolding;

        //Need Modify
        private void Start()
        {
            Input.simulateMouseWithTouches = false;
        }

        private void Update()
        {
            if (!IsDragActive)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                    _isHolding = true;
                }
                else if (Input.touchCount > 0)
                {
                    ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                    _isHolding = true;
                }
            }
            else
            {
                if (!_isHolding || Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>());
                    return;
                }
                /*if (Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>());
                    return;
                }*/


                if (Input.GetMouseButton(0) && _isHolding)
                {
                    DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                }
                if (Input.touchCount > 0 
                    && Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary 
                    && _isHolding)
                {
                    DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isHolding = false;
            }
        }
    }
}
