using System;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Animation
{
    public static class RectTransformAnimExtensions
    {
        public static Tweener AnchorTo(this RectTransform t, Vector3 position)
        {
            return AnchorTo(t, position, Tweener.DefaultDuration);
        }

        public static Tweener AnchorTo(this RectTransform t, Vector3 position, float duration)
        {
            return AnchorTo(t, position, duration, Tweener.DefaultEquation);
        }

        public static Tweener AnchorTo(this RectTransform t, Vector3 position, float duration,
                                       Func<float, float, float, float> equation)
        {
            RectTransformAnchorPositionTweener tweener = t.gameObject.AddComponent<RectTransformAnchorPositionTweener>();
            tweener.startValue = t.anchoredPosition;
            tweener.endValue = position;
            tweener.easingControl.duration = duration;
            tweener.easingControl.equation = equation;
            tweener.easingControl.Play();
            return tweener;
        }
    }
}
