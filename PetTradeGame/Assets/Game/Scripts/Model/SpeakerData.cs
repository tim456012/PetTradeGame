using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [Serializable]
    public class SpeakerData
    {
        public string speakerName;
        public string dialogueId;
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