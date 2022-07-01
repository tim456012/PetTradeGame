using System;
using System.Collections;
using Game.Scripts.Common.Animation;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class ConversationController : MonoBehaviour
    {
        [SerializeField] private ConversationPanel leftPanel;
        [SerializeField] private ConversationPanel rightPanel;
        [SerializeField] private ConversationPanel centerPanel;
        
        private Canvas _canvas;
        private IEnumerator _conversation;
        private Tweener _transition;

        private const string ShowTop = "Show Top";
        private const string ShowBottom = "Show Bottom";
        private const string ShowCenter = "Show Center";
        private const string HideTop = "Hide Top";
        private const string HideBottom = "Hide Bottom";
        private const string HideCenter = "Hide Center";

        public static event EventHandler CompleteEvent;

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            if (leftPanel.panel.CurrentPosition == null)
            {
                leftPanel.panel.SetPosition(HideBottom, false);
            }

            if (rightPanel.panel.CurrentPosition == null)
            {
                rightPanel.panel.SetPosition(HideBottom, false);
            }

            if (centerPanel.panel.CurrentPosition == null)
            {
                centerPanel.panel.SetPosition(HideBottom, false);
            }
        }

        public void Show(ConversationData data)
        {
            _canvas.gameObject.SetActive(true);
            if (data == null)
                ConversationCompleted();
            
            _conversation = Sequence(data);
            _conversation.MoveNext();
        }

        public void Next()
        {
            if (_conversation == null || _transition != null)
                return;

            _conversation.MoveNext();
        }

        private IEnumerator Sequence(ConversationData data)
        {
            if (data == null)
            {
                yield return null;
                _canvas.gameObject.SetActive(false);
                CompleteEvent?.Invoke(this, EventArgs.Empty);
                _conversation = null;
                yield break;
            }

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
                while (presenter.MoveNext())
                    yield return null;

                MovePanel(currentPanel, hide);
                _transition.easingControl.completedEvent += delegate
                {
                    _conversation.MoveNext();
                };

                yield return null;
            }
            ConversationCompleted();
        }

        private void MovePanel(ConversationPanel conversationPanel, string pos)
        {
            _transition = conversationPanel.panel.SetPosition(pos, true);
            _transition.easingControl.duration = 0.5f;
            _transition.easingControl.equation = EasingEquations.EaseOutQuad;
        }

        public void ConversationCompleted()
        {
            _canvas.gameObject.SetActive(false);
            CompleteEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
