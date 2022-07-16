using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public class PossessionLicenseRecipe : ScriptableObject
    {
        private static readonly Regex Regex = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);

        public List<PossessionLicenseData> possessionLicenseData = new List<PossessionLicenseData>();

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

            var data = new PossessionLicenseData(lines[0])
            {
                licenseNumber = new List<string>(),
                deadline = new List<string>(),
                name = new List<string>(),
                id = new List<string>(),
                businessNumber = new List<string>(),
                animalName = new List<string>(),
                contract = new List<string>(),
                objective = new List<string>(),
                original = new List<string>(),
                stampSign = new List<string>()
            };

            for (int i = 1; i <= 2; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.licenseNumber.Add(lines[i]);
            }

            for (int i = 3; i <= 4; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.deadline.Add(lines[i]);
            }

            for (int i = 5; i <= 6; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.name.Add(lines[i]);
            }

            for (int i = 7; i <= 8; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.id.Add(lines[i]);
            }

            for (int i = 9; i <= 10; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.businessNumber.Add(lines[i]);
            }

            for (int i = 11; i <= 12; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalName.Add(lines[i]);
            }

            for (int i = 13; i <= 14; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.contract.Add(lines[i]);
            }

            for (int i = 15; i <= 16; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.objective.Add(lines[i]);
            }

            for (int i = 17; i <= 18; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.original.Add(lines[i]);
            }

            for (int i = 19; i <= 20; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                data.stampSign.Add(lines[i]);
            }
            possessionLicenseData.Add(data);
        }
    }

    [Serializable]
    public class PossessionLicenseData
    {
        public string animalId;

        [Header("License Number")]
        public List<string> licenseNumber;

        [Header("Deadline")]
        public List<string> deadline;

        [Header("Name")]
        public List<string> name;

        [Header("ID")]
        public List<string> id;

        [Header("BusinessNumber")]
        public List<string> businessNumber;

        [Header("Animal Name")]
        public List<string> animalName;

        [Header("Contract")]
        public List<string> contract;

        [Header("Objective")]
        public List<string> objective;

        [Header("Original")]
        public List<string> original;

        [Header("Stamp & Sign Position")]
        public List<string> stampSign;

        public PossessionLicenseData() { }

        public PossessionLicenseData(string id)
        {
            animalId = id;
        }
    }
}