using System;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Animation
{
    public class TransformScaleTweener : Vector3Tweener
    {
        protected override void OnUpdate(object sender, EventArgs e)
        {
            base.OnUpdate(sender, e);
            transform.localScale = currentValue;
        }
    }
}
