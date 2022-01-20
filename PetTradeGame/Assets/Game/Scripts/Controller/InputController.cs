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

        private void Update()
        {
            if (IsDragActive)
            {
                DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                }

                //If player either clicked the mouse or lifted the finger during dragging, invoke the drop event.
                if (Input.GetMouseButtonDown(0))
                {
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>());
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                }
                else if (Input.touchCount > 0)
                {
                    ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                }
            }
        }
    }
}
