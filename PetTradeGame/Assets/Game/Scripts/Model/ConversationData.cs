using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Game.Scripts.Enum;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    public class ConversationData : ScriptableObject
    {
        public string description;
        public List<DialogueData> dialogueList = new();
        public List<SpeakerData> speakerList;

        private static readonly Regex regex = new("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);

        public void Load(string line)
        {
            //string[] lines = line.Split(",");
            List<string> lines = new List<string>();
            foreach (Match match in regex.Matches(line))
            {
                string current = match.Value;
                if (0 == current.Length)
                {
                    lines.Add("");
                }
                lines.Add(current.TrimStart(','));
            }
            
            DialogueData data  = new DialogueData(lines[0])
            {
                dialogueId = Convert.ToInt32(lines[1]),
                eventType = Convert.ToString(lines[2]) switch
                {
                    "Cutscene" => DialogueEvent.CutScenes,
                    "Intro" => DialogueEvent.Intro,
                    "Middle" => DialogueEvent.Middle,
                    "Outro" => DialogueEvent.Outro,
                    _ => DialogueEvent.Other,
                },
                speaker = Resources.Load<Sprite>($"Test/{lines[3]}"),
                anchor = Convert.ToString(lines[4]) switch
                {
                    "Upper Left" => TextAnchor.UpperLeft,
                    "Upper Center" => TextAnchor.UpperCenter,
                    "Upper Right" => TextAnchor.UpperRight,
                    "Middle Left" => TextAnchor.MiddleLeft,
                    "Middle Center" => TextAnchor.MiddleCenter,
                    "Middle Right" => TextAnchor.MiddleRight,
                    "Lower Left" => TextAnchor.LowerLeft,
                    "Lower Center" => TextAnchor.MiddleCenter,
                    "Lower Right" => TextAnchor.MiddleRight,
                    _ => TextAnchor.UpperLeft,
                }
            };

            if (lines.Count > 5)
            {
                data.messages = new List<string>(lines.Count - 5);
                for (int i = 5; i < lines.Count; ++i)
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
