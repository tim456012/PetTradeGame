using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Game.Scripts.Controller;
using Game.Scripts.EventArgs;

public class inputTest : MonoBehaviour
{
    private void OnEnable()
    {
        InputController.clickEvent += OnClickedEvent;
    }

    private void OnDisable()
    {
        InputController.clickEvent -= OnClickedEvent;
    }

    private void OnClickedEvent(object sender, InfoEventArgs<int> e)
    {
        Debug.Log("Fire" + e.info);
    }
}
