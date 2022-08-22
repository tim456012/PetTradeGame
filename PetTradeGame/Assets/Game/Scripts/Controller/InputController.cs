using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class InputController : MonoBehaviour
    {
        public static bool IsDragActive = false, IsPause;
        public static event EventHandler<InfoEventArgs<Vector3>> ClickedEvent, DraggingEvent, DroppingEvent;
        
        private bool _isHolding;
        
        private void Update()
        {
            //Invoke general ClickedEvent when player is not doing dragging.
            //Mobile Version
 #if UNITY_ANDROID || UNITY_IOS

            if (Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Began)
            {
                ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                return;
            }
#endif
            
            //PC Version

#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                ClickedEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
#endif
            
            if (IsPause)
                return;
            
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                _isHolding = true;

            if (Input.GetMouseButtonUp(0) || Input.touchCount < 0)
                _isHolding = false;

            //Check Input
            CheckInput(IsDragActive);
        }
        
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
#if UNITY_ANDROID || UNITY_IOS
                    case false when Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Ended or TouchPhase.Canceled:
                        DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        return;

                    //Invoke HoldingEvent when player still holding their input.
                    case true when Input.touchCount > 0 && Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary:
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.GetTouch(0).position));
                        return;
#endif

                    //PC Version
#if UNITY_STANDALONE || UNITY_EDITOR
                    case false:
                        DroppingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                        return;

                    case true:
                        DraggingEvent?.Invoke(this, new InfoEventArgs<Vector3>(Input.mousePosition));
                        return;
#endif
                }
            }
        }
    }
}