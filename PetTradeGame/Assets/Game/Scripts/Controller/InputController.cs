using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
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
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                _isHolding = true;
            
            if (Input.GetMouseButtonUp(0))
                _isHolding = false;
            
            //Check Input
            checkInput(IsDragActive);
            

            
            /*switch (IsDragActive)
            {
                case false when Input.GetMouseButtonDown(0):
                    ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                    _isHolding = true;
                    break;

                case false:
                    if (Input.touchCount > 0)
                    {
                        ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        _isHolding = true;
                    }
                    break;

                default:
                {
                    //Check the input first. If users stop dragging, invoke the drop event.
                    if (!_isHolding || Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled)
                    {
                        DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>());
                        return;
                    }

                    //Continues invoking drag event to sending the input position to subscribers.
                    if (Input.GetMouseButton(0) && _isHolding)
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));

                    //Mobile
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary && _isHolding)
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                    break;
                }
            }*/


        }

        private void checkInput(bool dragging)
        {
            switch (dragging)
            {
                //Check the input first. If users stop dragging, invoke the drop event.
                case false when !_isHolding || Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled:
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>());
                    return;
                //Continues invoking drag event to sending the input position to subscribers.
                case true when _isHolding:
                {
                    //Mobile
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary && _isHolding)
                    {
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        return;
                    }
                
                    DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                    break;
                }
            }
        }
    }
}
