using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class SpeakerData
    {
        public List<string> messages;
        public Sprite speaker;
        public TextAnchor anchor;
    }
}
