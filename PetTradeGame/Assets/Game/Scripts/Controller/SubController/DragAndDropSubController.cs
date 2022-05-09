using Game.Scripts.Common.Animation;
using Game.Scripts.EventArguments;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.SubController
{
    public class DragAndDropSubController : MonoBehaviour
    {
        private Vector2 _screenPos;
        private Vector3 _gameWorldPos;
        private Vector3 _originalPos;

        public GameObject TargetObj { get; set; }

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
            _gameWorldPos = Camera.main!.ScreenToWorldPoint(_screenPos);

            var hit = Physics2D.Raycast(_gameWorldPos, Vector2.zero);
            if (hit.collider == null)
                return;

            TargetObj = hit.transform.gameObject;
            //Debug.Log($"GameObject {TargetObj.name} selected.");
            var temp = TargetObj.GetComponent<EntityAttribute>();
            if (!temp.isDraggable)
                return;

            _originalPos = TargetObj.transform.localPosition;
            InputController.IsDragActive = true;
        }

        private void OnDragEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Dragging Event invoke.");
            if (!TargetObj || TargetObj == null)
                return;

            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main!.ScreenToWorldPoint(_screenPos);
            TargetObj.transform.localPosition = new Vector2(_gameWorldPos.x, _gameWorldPos.y);
        }

        private void OnDropEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Drop Event invoke.");
            if (!TargetObj || TargetObj == null)
                return;

            var cameraWorld = Camera.main!.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            InputController.IsDragActive = false;
            if (TargetObj.transform.localPosition.x > cameraWorld.x / 1.13
                || TargetObj.transform.localPosition.y > cameraWorld.y / 1.13
                || TargetObj.transform.localPosition.x < -cameraWorld.x / 1.13
                || TargetObj.transform.localPosition.y < -cameraWorld.y / 1.13)
            {
                TargetObj.transform.MoveToLocal(_originalPos, 1f, EasingEquations.EaseInOutExpo);
            }

            TargetObj = null;
        }
    }
}
