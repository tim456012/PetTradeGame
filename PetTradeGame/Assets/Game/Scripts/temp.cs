using Assets.Game.Scripts.Common.Animation;
using UnityEngine;

public class temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tweener tweener =  transform.MoveTo(new Vector3(5, 0, 0), 3f, EasingEquations.EaseInOutQuad);
        tweener.easingControl.loopCount = -1;
        tweener.easingControl.loopType = EasingControl.LoopType.PingPong;
    }

}
