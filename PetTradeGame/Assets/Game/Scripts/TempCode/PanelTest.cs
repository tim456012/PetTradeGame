using Game.Scripts.Common.Animation;
using Game.Scripts.Common.UI;
using UnityEngine;

namespace Game.Scripts.TempCode
{
    public class PanelTest : MonoBehaviour
    {
        private const string Show = "Show";
        private const string Hide = "Hide";
        private const string Center = "Center";
        private Panel panel;

        // Start is called before the first frame update
        private void Start()
        {
            panel = GetComponent<Panel>();
            var centerPos = new Panel.Position(Center, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter);
            panel.AddPosition(centerPos);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), Show))
            {
                panel.SetPosition(Show, true);
            }
            if (GUI.Button(new Rect(10, 50, 100, 30), Hide))
            {
                panel.SetPosition(Hide, true);
            }
            if (GUI.Button(new Rect(10, 90, 100, 30), Center))
            {
                Tweener tweener = panel.SetPosition(Center, true);
                tweener.easingControl.equation = EasingEquations.EaseInOutQuad;
            }
        }
    }
}