using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    public class ConversationController : MonoBehaviour
    {
        private const string ShowTop = "Show Top";
        private const string ShowBottom = "Show Bottom";
        private const string ShowCenter = "Show Center";
        private const string HideTop = "Hide Top";
        private const string HideBottom = "Hide Bottom";
        private const string HideCenter = "Hide Center";
        
        [SerializeField] private ConversationPanel leftPanel;
        [SerializeField] private ConversationPanel rightPanel;
        [SerializeField] private ConversationPanel centerPanel;

        private Canvas _canvas;
        private IEnumerator _conversation;
        private Tweener _transition;
        private SpeakerData[] _globalDialogue, _levelSpeakers;
        
        private int _r, _pr, _npc = 1;

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
        
        public void StartConversation(bool isCompliant = false)
        {
            var list = SplitArray(_levelSpeakers, isCompliant);
            Show(list);
        }

        public void StartTutorialConversation(int index = 0)
        {
            var list = SplitTutorialArray(_levelSpeakers, index);
            Show(list);
        }
        
        private void Show(SpeakerData[] data)
        {
            _canvas.gameObject.SetActive(true);
            if (data == null)
            {
                Debug.Log("Conversation Data is null");
                ConversationCompleted();
                return;
            }

            _conversation = Sequence(data);
            _conversation.MoveNext();
        }

        public void Next()
        {
            if (_conversation == null || _transition != null)
                return;

            _conversation.MoveNext();
        }

        private IEnumerator Sequence(IEnumerable<SpeakerData> data)
        {
            foreach (SpeakerData speakerData in data)
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

            _conversation = null;
            ConversationCompleted();
        }

        private void MovePanel(ConversationPanel conversationPanel, string pos)
        {
            _transition = conversationPanel.panel.SetPosition(pos, true);
            _transition.easingControl.duration = 0.5f;
            _transition.easingControl.equation = EasingEquations.EaseOutQuad;
        }

        private void ConversationCompleted()
        {
            _canvas.gameObject.SetActive(false);
            CompleteEvent?.Invoke(this, EventArgs.Empty);
        }

        public void LoadGlobalDialogue(List<SpeakerData> speakerList)
        {
            _globalDialogue = speakerList.ToArray();
        }
        
        public void LoadTutorialDialogue(List<SpeakerData> speakerList)
        {
            _levelSpeakers = speakerList.ToArray();
        }
        
        public void LoadLevelDialogue(List<SpeakerData> speakerList)
        {
            _levelSpeakers = speakerList.ToArray();
        }
        
        private SpeakerData[] SplitArray(SpeakerData[] data, bool makeComplaintList)
        {
            var stack = new Stack<SpeakerData>();
            if (!makeComplaintList)
            {
                //Add first player dialogue as default
                stack.Push(_globalDialogue[0]);
                //Find and add npc response dialogue to stack. If no npc response, just return the array
                SpeakerData response = Array.Find(data, speaker => speaker.dialogueId.StartsWith($"R{_r}") && speaker.dialogueId.Contains($"N{_npc.ToString()}"));
                if (response != null)
                {
                    stack.Push(response);
                    //if the npc response is also need player response, find and add it to the stack
                    if (response.dialogueId.Contains("_E"))
                    {
                        SpeakerData playerResponse = Array.Find(data, speaker => speaker.dialogueId.StartsWith($"PR{_pr}"));
                        stack.Push(playerResponse);
                        _pr++;
                        _r++;
                        
                        //If the player response is need npc response again, find and add it to the stack
                        if (playerResponse != null)
                        {
                            SpeakerData npcResponse = Array.Find(data, speaker => speaker.dialogueId.StartsWith($"R{_r}") && speaker.dialogueId.EndsWith($"N{_npc.ToString()}"));
                            if (npcResponse != null)
                                stack.Push(npcResponse);
                            _r = 0;
                        }
                        else
                            Debug.Log("No player response dialogue found");
                    }
                    else
                        Debug.Log("No player response dialogue found");
                }
                else
                    Debug.Log("No response dialogue found");

                _npc++;
                var array = stack.ToArray();
                Array.Reverse(array);
                stack.Clear();
                return array;
            }
            
            var num = _npc - 1;
            SpeakerData ask = Array.Find(data, speaker => speaker.dialogueId.StartsWith($"A0") && speaker.dialogueId.EndsWith($"N{num.ToString()}"));
            if (ask != null)
            {
                stack.Push(ask);
                var p = Random.Range(2, 6);
                SpeakerData response = Array.Find(_globalDialogue, speaker => speaker.dialogueId.EndsWith($"P{p}"));
                stack.Push(response);
                    
                var array = stack.ToArray();
                Array.Reverse(array);
                return array;
            }
                
            Debug.Log("No ask dialogue found");
            return null;
        }

        private SpeakerData[] SplitTutorialArray(SpeakerData[] data, int index)
        {
            var stack = new Stack<SpeakerData>();
            switch (index)
            {
                case 0:
                    stack.Push(data[0]);
                    stack.Push(data[1]);
                    break;
                case 1:
                    stack.Push(data[2]);
                    stack.Push(data[3]);
                    break;
                case 2:
                    stack.Push(data[4]);
                    break;
                case 3:
                    stack.Push(data[5]);
                    break;
                case 4:
                    stack.Push(data[6]);
                    break;
                case 5:
                    stack.Push(data[7]);
                    break;
            }
            
            var array = stack.ToArray();
            Array.Reverse(array);
            stack.Clear();
            return array;
        }
        
        public void Release()
        {
            _r = 0;
            _pr = 0;
            _npc = 1;
        }
    }
}