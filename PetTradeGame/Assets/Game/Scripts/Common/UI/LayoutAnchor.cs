using Assets.Game.Scripts.Common.Animation;
using UnityEngine;

namespace Assets.Game.Scripts.Common.UI
{
    /// <summary>
    /// Provide an easy way to modify the relationship of RectTransform via the inspector.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class LayoutAnchor : MonoBehaviour
    {
        private RectTransform rectTransform; //itself
        private RectTransform parentRT;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            parentRT = transform.parent as RectTransform;
            if (parentRT == null)
            {
                Debug.LogError("This component requires a RectTransform parent object to work.", gameObject);
            }
        }

        //Get the general offsets based on the location of the anchor and the size of
        //RectTransform rect that we want
        private Vector2 GetPosition(RectTransform rt, TextAnchor anchor)
        {
            Vector2 retValue = Vector2.zero;

            switch (anchor)
            {
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    retValue.x += rt.rect.width * 0.5f;
                    break;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    retValue.x += rt.rect.width;
                    break;
            }

            switch (anchor)
            {
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    retValue.y += rt.rect.height * 0.5f;
                    break;
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    retValue.y += rt.rect.height;
                    break;
            }
            return retValue;
        }

        //Based on the anchor points we specify to calculating the correct position of the RectTransform (pixel-prefect)
        public Vector2 AnchorPosition(TextAnchor anchor, TextAnchor parentAnchor, Vector2 offset)
        {
            Vector2 theOffset = GetPosition(rectTransform, anchor);
            Vector2 parentOffset = GetPosition(parentRT, parentAnchor);
            Vector2 anchorCenter = new Vector2(Mathf.Lerp(rectTransform.anchorMin.x, rectTransform.anchorMax.x, rectTransform.pivot.x), 
                Mathf.Lerp(rectTransform.anchorMin.y, rectTransform.anchorMax.y, rectTransform.pivot.y));
            Vector2 anchorOffset = new Vector2(parentRT.rect.width * anchorCenter.x, parentRT.rect.height * anchorCenter.y);
            Vector2 pivotOffset = new Vector2(rectTransform.rect.width * rectTransform.pivot.x,
                rectTransform.rect.height * rectTransform.pivot.y);
            Vector2 pos = parentOffset - anchorOffset - theOffset + pivotOffset + offset;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);
            return pos;
        }
    
        /// <summary>
        /// Determine where to place the RectTransform.
        /// </summary>
        /// <param name="anchor">The anchor of GameObject itself.</param>
        /// <param name="parentAnchor">The anchor of parent GameObject.</param>
        /// <param name="offset">The offset of the RectTransform that we want.</param>
        public void SnapToAnchorPosition(TextAnchor anchor, TextAnchor parentAnchor, Vector2 offset)
        {
            rectTransform.anchoredPosition = AnchorPosition(anchor, parentAnchor, offset);
        }
    
        /// <summary>
        /// Moving RectTransform into position with animation.
        /// </summary>
        /// <param name="anchor">The anchor of GameObject itself.</param>
        /// <param name="parentAnchor">The anchor of parent GameObject.</param>
        /// <param name="offset">The offset of the RectTransform that we want.</param>
        /// <returns></returns>
        public Tweener MoveToAnchorPosition(TextAnchor anchor, TextAnchor parentAnchor, Vector2 offset)
        {
            return rectTransform.AnchorTo(AnchorPosition(anchor, parentAnchor, offset));
        }
    }
}
