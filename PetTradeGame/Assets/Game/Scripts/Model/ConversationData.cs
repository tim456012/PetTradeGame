using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "ConversationData", menuName = "ScriptableObject/Conversation Data")]
    public class ConversationData : ScriptableObject
    {

        private static readonly Regex Regex = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);
        public string description;
        public List<SpeakerData> speakerList = new List<SpeakerData>();

        public void Load(string line)
        {
            List<string> lines = new List<string>();
            foreach (Match match in Regex.Matches(line))
            {
                string current = match.Value;
                if (0 == current.Length)
                    lines.Add("");
                lines.Add(current.Trim(',', '"'));
            }

            var data = new SpeakerData(lines[0])
            {
                dialogueId = Convert.ToInt32(lines[1]),
                speaker = Resources.Load<Sprite>($"NPC/{lines[2]}"),
                anchor = Convert.ToString(lines[3]) switch
                {
                    "Upper Left" => TextAnchor.UpperLeft,
                    "Upper Center" => TextAnchor.UpperCenter,
                    "Upper Right" => TextAnchor.UpperRight,
                    "Middle Left" => TextAnchor.MiddleLeft,
                    "Middle Center" => TextAnchor.MiddleCenter,
                    "Middle Right" => TextAnchor.MiddleRight,
                    "Lower Left" => TextAnchor.LowerLeft,
                    "Lower Center" => TextAnchor.LowerCenter,
                    "Lower Right" => TextAnchor.LowerRight,
                    _ => TextAnchor.UpperLeft
                }
            };

            if (lines.Count >= 4)
            {
                data.messages = new List<string>(lines.Count - 4);
                for (int i = 4; i < lines.Count; ++i)
                {
                    string text = Convert.ToString(lines[i]);
                    if (string.IsNullOrEmpty(text))
                        continue;
                    data.messages.Add(text);
                }
            }
            speakerList.Add(data);
        }
    }
}