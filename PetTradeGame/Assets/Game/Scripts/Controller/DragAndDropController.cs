using Game.Scripts.Common.Animation;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Scripts.Controller
{
    public class DragAndDropController : MonoBehaviour
    {
        private Vector3 _gameWorldPos;
        private Vector3 _originalPos;
        private Vector2 _screenPos;

        public GameObject TargetObj { get; set; }
        public GameObject LastObj { get; set; }

        private void OnEnable()
        {
            InputController.ClickedEvent += OnClickedEvent;
            InputController.DraggingEvent += OnDragEvent;
            InputController.DroppingEvent += OnDropEvent;
            
            GamePlayController.ForceDropEvent += OnDropEvent;
        }

        private void OnDisable()
        {
            InputController.ClickedEvent -= OnClickedEvent;
            InputController.DraggingEvent -= OnDragEvent;
            InputController.DroppingEvent -= OnDropEvent;
            
            GamePlayController.ForceDropEvent -= OnDropEvent;
        }

        private void OnClickedEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Click Event invoke.");
            if(InputController.IsPause)
                return;
            
            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main!.ScreenToWorldPoint(_screenPos);

            RaycastHit2D hit = Physics2D.Raycast(_gameWorldPos, Vector2.zero);
            if (hit.collider == null)
                return;

            TargetObj = hit.transform.gameObject;
            //Debug.Log($"GameObject {TargetObj.name} selected.");
            var temp = TargetObj.GetComponent<EntityAttribute>();
            if (!temp.isDraggable)
                return;

            if (temp.isDocument || temp.GetComponent<LicenseInfo>())
            {
                var sg = TargetObj.GetComponent<SortingGroup>();
                sg.sortingOrder = 1;
            }
            else
            {
                temp.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "SelectedObjects";
            }

            _originalPos = TargetObj.transform.localPosition;
            InputController.IsDragActive = true;
        }

        private void OnDragEvent(object sender, InfoEventArgs<Vector3> e)
        {
            if (!TargetObj)
            {
                LastObj = null;
                TargetObj = null;
                return;
            }

            //Debug.Log($"Dragging Event invoke.");
            _screenPos = new Vector2(e.info.x, e.info.y);
            _gameWorldPos = Camera.main!.ScreenToWorldPoint(_screenPos);
            TargetObj.transform.localPosition = new Vector2(_gameWorldPos.x, _gameWorldPos.y);
        }

        private void OnDropEvent(object sender, InfoEventArgs<Vector3> e)
        {
            //Debug.Log($"Drop Event invoke.");
            if (!TargetObj || TargetObj == null)
                return;

            LastObj = TargetObj;
            if (TargetObj.GetComponent<EntityAttribute>().isDocument || TargetObj.GetComponent<LicenseInfo>())
            {
                var sg = TargetObj.GetComponent<SortingGroup>();
                sg.sortingOrder = 0;
            }
            else
            {
                TargetObj.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            }

            Vector3 cameraWorld = Camera.main!.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
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