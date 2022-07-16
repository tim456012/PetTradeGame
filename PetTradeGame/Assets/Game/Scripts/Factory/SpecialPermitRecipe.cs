using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public class SpecialPermitRecipe : ScriptableObject
    {
        private static readonly Regex Regex = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);

        public List<SpecialPermitData> specialPermitData = new List<SpecialPermitData>();

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

            var data = new SpecialPermitData(lines[0])
            {
                locationName = new List<string>(),
                line1Position = lines[3] switch
                {
                    " " => SpecialPermitData.SpecialPermitPosition.None,
                    "C1" => SpecialPermitData.SpecialPermitPosition.CirclePos1,
                    "C2" => SpecialPermitData.SpecialPermitPosition.CirclePos2,
                    "C3" => SpecialPermitData.SpecialPermitPosition.CirclePos3,
                    _ => SpecialPermitData.SpecialPermitPosition.None
                },
                objective = new List<string>(),
                businessNumber = new List<string>(),
                deadline = new List<string>(),
                animalName = new List<string>(),
                animalCount = new List<string>(),
                animalFeature = new List<string>(),
                stampSign = new List<string>()
            };

            for (int i = 1; i <= 2; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.locationName.Add(lines[i]);
            }

            for (int i = 4; i <= 5; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.objective.Add(lines[i]);
            }

            for (int i = 6; i <= 7; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.businessNumber.Add(lines[i]);
            }

            for (int i = 8; i <= 9; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.deadline.Add(lines[i]);
            }

            for (int i = 10; i <= 11; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalName.Add(lines[i]);
            }

            for (int i = 12; i <= 13; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalCount.Add(lines[i]);
            }

            for (int i = 14; i <= 15; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalFeature.Add(lines[i]);
            }

            for (int i = 16; i <= 17; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.stampSign.Add(lines[i]);
            }

            specialPermitData.Add(data);
        }
    }

    [Serializable]
    public class SpecialPermitData
    {
        public enum SpecialPermitPosition
        {
            None,
            CirclePos1,
            CirclePos2,
            CirclePos3
        }

        public string animalId;

        [Header("Location Name")]
        public List<string> locationName;

        [Header("Line 1")]
        public SpecialPermitPosition line1Position;

        [Header("Objective")]
        public List<string> objective;

        [Header("Business Number")]
        public List<string> businessNumber;

        [Header("Deadline")]
        public List<string> deadline;

        [Header("Animal Name")]
        public List<string> animalName;

        [Header("Animal Count")]
        public List<string> animalCount;

        [Header("Animal Feature")]
        public List<string> animalFeature;

        [Header("Stamp & Sign Position")]
        public List<string> stampSign;

        public SpecialPermitData() { }

        public SpecialPermitData(string id)
        {
            animalId = id;
        }
    }
}