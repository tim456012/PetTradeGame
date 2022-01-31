using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    public class ConversationData : ScriptableObject
    {
        public string Description;
        public List<DialogueData> dialogueList = new();
        public List<SpeakerData> speakerList;

        public void Load(string line)
        {
            string[] lines = line.Split(",");
            DialogueData data  = new DialogueData();

            name = lines[0];
            data.dialogueId = Convert.ToInt32(lines[1]);
            data.dialogueType = Convert.ToString(lines[2]);

            if (lines.Length > 3)
            {
                data.messages = new List<string>(lines.Length - 3);
                for (int i = 3; i < lines.Length; ++i)
                {
                    data.messages.Add(Convert.ToString(lines[i]));
                }
            }
            else
            {
                data.messages = new List<string> { Convert.ToString(lines[3]) };
            }

            dialogueList.Add(data);
        }
    }
}
