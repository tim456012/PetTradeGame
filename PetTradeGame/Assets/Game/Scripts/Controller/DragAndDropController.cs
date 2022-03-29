using System;
using Game.Scripts.Common.Animation;
using Game.Scripts.EventArguments;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class DragAndDropController : MonoBehaviour
    {
        private Vector2 _screenPos;
        private Vector3 _gameWorldPos;
        private Vector3 _originalPos;
        private GameObject _targetObj;

        private void OnEnable()
        {
            InputController.ClickedEvent += OnClickedEvent;
            InputController.DraggingEvent += OnDragEvent;
            InputController.DroppingEvent += OnDropEvent;
        }

        private void OnDisable()
        {
            InputController.ClickedEvent -= OnClickedEvent;
            InputController.DraggingEvent -= OnDragEvent;
            InputController.DroppingEvent -= OnDropEvent;
        }

        private void OnClickedEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Click Event invoke.");

            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main.ScreenToWorldPoint(_screenPos);

            RaycastHit2D hit = Physics2D.Raycast(_gameWorldPos, Vector2.zero);
            if (hit.collider == null) 
                return;

            _targetObj = hit.transform.gameObject;
            //Debug.Log($"GameObject {_targetObj.name} selected.");
            EntityAttribute temp = _targetObj.GetComponent<EntityAttribute>();
            if (!temp.isDraggable)
                return;

            _originalPos = _targetObj.transform.localPosition;
            InputController.IsDragActive = true;
        }

        private void OnDragEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Dragging Event invoke.");

            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main.ScreenToWorldPoint(_screenPos);
            //_targetObj.transform.MoveToLocal(new Vector2(_gameWorldPos.x, _gameWorldPos.y), 0.2f, EasingEquations.EaseOutSine);
            _targetObj.transform.localPosition = new Vector2(_gameWorldPos.x, _gameWorldPos.y);
        }

        private void OnDropEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Drop Event invoke.");
            
            Vector3 cameraWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            InputController.IsDragActive = false;
            if (_targetObj.transform.localPosition.x > (cameraWorld.x / 1.13)
                || _targetObj.transform.localPosition.y > (cameraWorld.y / 1.13)
                || _targetObj.transform.localPosition.x < -cameraWorld.x / 1.13
                || _targetObj.transform.localPosition.y < -cameraWorld.y / 1.13)
            {
                _targetObj.transform.MoveToLocal(_originalPos, 2f, EasingEquations.EaseInOutExpo);
            }
           
            _targetObj = null;
        }
    }
}
