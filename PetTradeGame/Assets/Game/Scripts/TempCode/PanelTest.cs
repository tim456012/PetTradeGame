using System;
using System.Collections;
using Assets.Game.Scripts.Common.Animation;
using Assets.Game.Scripts.Common.UI;
using UnityEngine;

public class PanelTest : MonoBehaviour
{
    private Panel panel;
    private const string Show = "Show";
    private const string Hide = "Hide";
    private const string Center = "Center";
    
    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponent<Panel>();
        Panel.Position centerPos = new Panel.Position(Center, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter);
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
