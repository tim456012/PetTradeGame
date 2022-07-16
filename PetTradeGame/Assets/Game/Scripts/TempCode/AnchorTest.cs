using System.Collections;
using Game.Scripts.Common.Animation;
using Game.Scripts.Common.UI;
using UnityEngine;

namespace Game.Scripts.TempCode
{
    public class AnchorTest : MonoBehaviour
    {
        [SerializeField] private bool animated;
        [SerializeField] private float delay = 0.5f;

        private IEnumerator Start()
        {
            var anchor = GetComponent<LayoutAnchor>();
            while (true)
            {
                for (int i = 0; i < 9; ++i)
                {
                    for (int j = 0; j < 9; ++j)
                    {
                        var a1 = (TextAnchor)i;
                        var a2 = (TextAnchor)j;
                        Debug.Log($"A1 : [{a1}], A2 : [{a2}]");

                        if (animated)
                        {
                            Tweener t = anchor.MoveToAnchorPosition(a1, a2, Vector2.zero);
                            while (t != null)
                            {
                                yield return null;
                            }
                        }
                        else
                        {
                            anchor.SnapToAnchorPosition(a1, a2, Vector2.zero);
                        }
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
        }
    }
}