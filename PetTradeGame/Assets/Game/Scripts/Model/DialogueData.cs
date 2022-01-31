using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Game.Scripts.Enum;

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class DialogueData
    {
        public string name;
        public int dialogueId;
        public DialogueEvent eventType;
        public List<string> messages;
        public Sprite speaker;
        public TextAnchor anchor;
        
        public DialogueData()
        {
            
        }
        
        public DialogueData(string name)
        {
            this.name = name;
        }
    }
}

