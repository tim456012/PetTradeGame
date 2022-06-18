using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public class HealthCertificateRecipe : ScriptableObject
    { 
        private readonly static Regex Regex = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);
        
        public List<HealthCertificateData> healthCertificateData = new List<HealthCertificateData>();

        public void Load(string line)
        {
            var lines = new List<string>();
            foreach (Match match in Regex.Matches(line))
            {
                string current = match.Value;
                if (0 == current.Length)
                    lines.Add("");
                lines.Add(current.Trim(',', '"'));
            }

            var data = new HealthCertificateData(lines[0])
            {
                isOfficial = lines[1].Equals("Yes"),
                isLocal = lines[2].Equals("Yes"),
                animalName = new List<string>(),
                animalMark = new List<string>(),
                date = new List<string>(),
                stampSign = new List<string>()
            };

            for (int i = 3; i <= 4; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalName.Add(lines[i]);
            }
            
            for (int i = 5; i <= 6; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.animalMark.Add(lines[i]);
            }
            
            for (int i = 7; i <= 8; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.date.Add(lines[i]);
            }
            
            for (int i = 9; i <= 10; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.stampSign.Add(lines[i]);
            }
            
            healthCertificateData.Add(data);
        }
    }

    [System.Serializable]
    public class HealthCertificateData
    {
        public string animalId;

        [Header("Line 1")]
        public bool isOfficial;

        [Header("Line 2")]
        public bool isLocal;

        [Header("Animal Name")]
        public List<string> animalName;

        [Header("Animal Mark")]
        public List<string> animalMark;

        [Header("Date")]
        public List<string> date;

        [Header("Stamp & Sign Position")]
        public List<string> stampSign;
        
        public HealthCertificateData() {}

        public HealthCertificateData(string id)
        {
            animalId = id;
        }
    }
}
