using Assets.Game.Scripts.EventArguments;
using Assets.Game.Scripts.Common.Animation;
using UnityEngine;

namespace Assets.Game.Scripts.Controller.Level_State
{
    public class PlayingState : GameLoopState
    {
        private Vector2 _screenPos;
        private Vector3 _gameWorldPos;
        private Vector3 _originalPos;
        private GameObject _targetObj;
        private DraggableTest _draggableTest;

        protected override void AddListeners()
        {
            base.AddListeners();
            InputController.DraggingEvent += OnDragEvent;
            InputController.DroppingEvent += OnDropEvent;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            InputController.DraggingEvent -= OnDragEvent;
            InputController.DroppingEvent -= OnDropEvent;
        }

        protected override void OnClick(object sender, InfoEventArgs<Vector3> e)
        {
            base.OnClick(sender, e);
            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main.ScreenToWorldPoint(_screenPos);

            RaycastHit2D hit = Physics2D.Raycast(_gameWorldPos, Vector2.zero);
            if (hit.collider == null)
                return;

            _targetObj = hit.transform.gameObject;
            Debug.Log($"GameObject {_targetObj.name} selected.");
            DraggableTest temp = _targetObj.GetComponent<DraggableTest>();
            if (!temp.IsDraggable)
                return;

            _originalPos = _targetObj.transform.localPosition;
            _draggableTest = temp;
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
        }
    }
}
