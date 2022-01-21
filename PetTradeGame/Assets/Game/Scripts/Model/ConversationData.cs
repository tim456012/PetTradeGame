using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    public class ConversationData : ScriptableObject
    {
        public List<SpeakerData> speakerList;

        public void Load(string line)
        {
            string[] lines = line.Split(",");
        }
    }
}
