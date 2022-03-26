using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts
{
    public class Slot : MonoBehaviour, IDropHandler
    {
        public GameObject item => transform.childCount > 0 ? transform.GetChild(0).gameObject : null;

        public void OnDrop(PointerEventData eventData)
        {
            if (item)
                return;
            DragHandler.targetObj.transform.SetParent(transform);
            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y)
                => x.HasChanged());
        }
    }
}
