﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.Common.Animation;
using Assets.Game.Scripts.Enum;
using Assets.Game.Scripts.Model;
using Assets.Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Assets.Game.Scripts.Controller
{
    public class ConversationController : MonoBehaviour
    {
        [SerializeField] private ConversationPanel leftPanel;
        [SerializeField] private ConversationPanel rightPanel;
        [SerializeField] private ConversationPanel centerPanel;

        private Canvas canvas;
        private IEnumerator conversation;
        private Tweener transition;

        private const string ShowTop = "Show Top";
        private const string ShowBottom = "Show Bottom";
        private const string ShowCenter = "Show Center";
        private const string HideTop = "Hide Top";
        private const string HideBottom = "Hide Bottom";
        private const string HideCenter = "Hide Center";

        public static event EventHandler CompleteEvent;

        private void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            if (leftPanel.panel.CurrentPosition == null)
            {
                leftPanel.panel.SetPosition(HideBottom, false);
            }

            if (rightPanel.panel.CurrentPosition == null)
            {
                rightPanel.panel.SetPosition(HideBottom, false );
            }

            if (centerPanel.panel.CurrentPosition == null)
            {
                centerPanel.panel.SetPosition(HideBottom, false);
            }
        }

        public void Show(ConversationData data)
        {
            canvas.gameObject.SetActive(true);
            conversation = Sequence(data);
            conversation.MoveNext();
        }

        public void Next()
        {
            if (conversation == null || transition != null)
                return;

            conversation.MoveNext();
        }

        private IEnumerator Sequence(ConversationData data)
        {
            foreach (var speakerData in data.speakerList)
            {
                ConversationPanel currentPanel = speakerData.anchor switch
                {
                    TextAnchor.UpperLeft or TextAnchor.MiddleLeft or TextAnchor.LowerLeft => leftPanel,
                    TextAnchor.UpperCenter or TextAnchor.MiddleCenter or TextAnchor.LowerCenter => centerPanel,
                    TextAnchor.UpperRight or TextAnchor.MiddleRight or TextAnchor.LowerRight => rightPanel,
                    _ => centerPanel
                };
                
                IEnumerator presenter = currentPanel.Display(speakerData);
                presenter.MoveNext();

                string show, hide;
                
                switch (speakerData.anchor)
                {
                    case TextAnchor.UpperLeft:
                    case TextAnchor.UpperCenter:
                    case TextAnchor.UpperRight: 
                        show = ShowTop;
                        hide = HideTop;
                        break;
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.MiddleCenter: 
                    case TextAnchor.MiddleRight:
                        show = ShowCenter;
                        hide = HideCenter;
                        break;
                    case TextAnchor.LowerLeft: 
                    case TextAnchor.LowerCenter: 
                    case TextAnchor.LowerRight:
                        show = ShowBottom;
                        hide = HideBottom;
                        break;
                    default:
                        show = ShowBottom;
                        hide = HideBottom;
                        break;
                }

                currentPanel.panel.SetPosition(hide, false);
                MovePanel(currentPanel, show);

                yield return null;
                while(presenter.MoveNext())
                    yield return null;

                MovePanel(currentPanel, hide);
                transition.easingControl.completedEvent += delegate
                {
                    conversation.MoveNext();
                };

                yield return null;
            }

            canvas.gameObject.SetActive(false);
            CompleteEvent?.Invoke(this, EventArgs.Empty);
        }

        private void MovePanel(ConversationPanel conversationPanel, string pos)
        {
            transition = conversationPanel.panel.SetPosition(pos, true);
            transition.easingControl.duration = 0.5f;
            transition.easingControl.equation = EasingEquations.EaseOutQuad;
        }
    }
}
