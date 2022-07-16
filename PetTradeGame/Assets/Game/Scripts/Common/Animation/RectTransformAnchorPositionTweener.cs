using System;
using UnityEngine;

namespace Game.Scripts.Common.Animation
{
    public class RectTransformAnchorPositionTweener : Vector3Tweener
    {
        private RectTransform _rectTransform;

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = transform as RectTransform;
        }

        protected override void OnUpdate(object sender, EventArgs e)
        {
            base.OnUpdate(sender, e);
            _rectTransform.anchoredPosition = currentValue;
        }
    }
}