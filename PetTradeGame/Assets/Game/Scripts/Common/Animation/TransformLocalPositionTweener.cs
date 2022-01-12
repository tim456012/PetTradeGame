using System;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Animation
{
    public class TransformLocalPositionTweener : Vector3Tweener
    {
        protected override void OnUpdate(object sender, EventArgs e)
        {
            base.OnUpdate(sender, e);
            transform.localPosition = currentValue;
        }
    }
}
