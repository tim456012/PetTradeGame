using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class DialogueData
    {
        public int dialogueId;
        public string dialogueType;
        public List<string> messages;
    }
}

