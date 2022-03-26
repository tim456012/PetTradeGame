using System;
using UnityEngine;

namespace Game.Scripts.Common.Animation
{
    public static class TransformExtensions
    {
        public static Tweener MoveTo(this Transform t, Vector3 position)
        {
            return MoveTo(t, position, Tweener.DefaultDuration);
        }

        public static Tweener MoveTo(this Transform t, Vector3 position, float duration)
        {
            return MoveTo(t, position, duration, Tweener.DefaultEquation);
        }

        public static Tweener MoveTo(this Transform t, Vector3 position, float duration,
                                     Func<float, float, float, float> equation)
        {
            TranformPositionTweener tweener = t.gameObject.AddComponent<TranformPositionTweener>();
            tweener.startValue = t.position;
            tweener.endValue = position;
            tweener.easingControl.duration = duration;
            tweener.easingControl.equation = equation;
            tweener.easingControl.Play();
            return tweener;
        }

        public static Tweener MoveToLocal(this Transform t, Vector3 position)
        {
            return MoveToLocal(t, position, Tweener.DefaultDuration);
        }

        public static Tweener MoveToLocal(this Transform t, Vector3 position, float duration)
        {
            return MoveToLocal(t, position, duration, Tweener.DefaultEquation);
        }

        public static Tweener MoveToLocal(this Transform t, Vector3 position, float duration,
                                          Func<float, float, float, float> equation)
        {
            TransformLocalPositionTweener tweener = t.gameObject.AddComponent<TransformLocalPositionTweener>();
            tweener.startValue = t.localPosition;
            tweener.endValue = position;
            tweener.easingControl.duration = duration;
            tweener.easingControl.equation = equation;
            tweener.easingControl.Play();
            return tweener;
        }

        public static Tweener ScaleTo(this Transform t, Vector3 scale)
        {
            return ScaleTo(t, scale, Tweener.DefaultDuration);
        }

        public static Tweener ScaleTo(this Transform t, Vector3 scale, float duration)
        {
            return ScaleTo(t, scale, duration, Tweener.DefaultEquation);
        }
        
        public static Tweener ScaleTo(this Transform t, Vector3 scale, float duration,
                                      Func<float, float, float, float> equation)
        {
            TransformScaleTweener tweener = t.gameObject.AddComponent<TransformScaleTweener>();
            tweener.startValue = t.localScale;
            tweener.endValue = scale;
            tweener.easingControl.duration = duration;
            tweener.easingControl.equation = equation;
            tweener.easingControl.Play();
            return tweener;
        }
    }
}
