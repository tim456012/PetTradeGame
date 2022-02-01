using System.Collections.Generic;
using Assets.Game.Scripts.Enum;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class SpeakerData
    {
        public string speakerName;
        public int dialogueId;
        public DialogueEvent eventType;
        public List<string> messages;
        public Sprite speaker;
        public TextAnchor anchor;

        public SpeakerData()
        {

        }

        public SpeakerData(string name)
        {
            speakerName = name;
        }
    }
}
