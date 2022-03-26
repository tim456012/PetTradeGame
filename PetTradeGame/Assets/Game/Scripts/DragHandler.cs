using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts
{
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static GameObject targetObj;
        private Vector3 startPosition;
        private Transform startParent;
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            targetObj = gameObject;
            startPosition = transform.position;
            startParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {
            targetObj = null;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (transform.parent != startParent)
            {
                transform.position = startPosition;
            }
        }

    }
}
