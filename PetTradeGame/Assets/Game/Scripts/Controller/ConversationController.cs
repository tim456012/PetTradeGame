using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.Common.Animation;
using Assets.Game.Scripts.Model;
using Assets.Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Assets.Game.Scripts.Controller
{
    public class ConversationController : MonoBehaviour
    {
        [SerializeField] private ConversationPanel leftPanel;
        [SerializeField] private ConversationPanel rightPanel;

        private Canvas canvas;
        private IEnumerator conversation;
        private Tweener transition;

        private const string ShowTop = "Show Top";
        private const string ShowBottom = "Show Bottom";
        private const string HideTop = "Hide Top";
        private const string HideBottom = "Hide Bottom";

        public static event EventHandler completeEvent;

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
            for (int i = 0; i < data.speakerList.Count; ++i)
            {
                SpeakerData speakerData = data.speakerList[i];
                ConversationPanel currentPanel = speakerData.anchor is TextAnchor.UpperLeft 
                    or TextAnchor.MiddleLeft 
                    or TextAnchor.LowerLeft ? leftPanel : rightPanel;
                IEnumerator presenter = currentPanel.Display(speakerData);
                presenter.MoveNext();

                string show, hide;
                if (speakerData.anchor is TextAnchor.UpperLeft
                    or TextAnchor.UpperCenter
                    or TextAnchor.UpperRight)
                {
                    show = ShowTop;
                    hide = HideTop;
                }
                else
                {
                    show = ShowBottom;
                    hide = HideBottom;
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
            completeEvent?.Invoke(this, EventArgs.Empty);
        }

        private void MovePanel(ConversationPanel conversationPanel, string pos)
        {
            transition = conversationPanel.panel.SetPosition(pos, true);
            transition.easingControl.duration = 0.5f;
            transition.easingControl.equation = EasingEquations.EaseOutQuad;
        }
    }
}
