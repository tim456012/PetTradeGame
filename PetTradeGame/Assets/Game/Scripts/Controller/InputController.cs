using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class InputController : MonoBehaviour
    {
        public static bool IsDragActive = false, IsPause;

        private bool _isHolding;

        private void Update()
        {
            if (IsPause)
                return;

            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                _isHolding = true;

            if (Input.GetMouseButtonUp(0) || Input.touchCount < 0)
                _isHolding = false;

            //Check Input
            CheckInput(IsDragActive);
        }
        public static event EventHandler<InfoEventArgs<Vector3>> ClickedEvent;
        public static event EventHandler<InfoEventArgs<Vector3>> DraggingEvent;
        public static event EventHandler<InfoEventArgs<Vector3>> DroppingEvent;

        private void CheckInput(bool dragging)
        {
            //Debug.Log(IsDragActive);
            //If player is dragging something, check their input first.
            if (dragging)
            {
                switch (_isHolding)
                {
                    //Invoke ReleaseEvent when player release their input
                    //Mobile Version
                    case false when Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled:
                        DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        return;
                    //PC Version
                    case false:
                        DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                        return;

                    //Invoke HoldingEvent when player still holding their input.
                    //Mobile Version
                    case true when Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary:
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        return;
                    //PC Version
                    case true:
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                        return;
                }
            }

            //Invoke general ClickedEvent when player is not doing dragging.
            //Mobile Version
            if (Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Began)
            {
                ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                return;
            }

            //PC Version
            if (Input.GetMouseButtonDown(0))
                ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
        }
    }
}