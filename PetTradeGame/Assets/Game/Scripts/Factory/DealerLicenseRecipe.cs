using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public class DealerLicenseRecipe : ScriptableObject
    {
        private readonly static Regex Regex = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);
        
        public List<DealerLicenseData> dealerLicenseData = new List<DealerLicenseData>();

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
            
            var data = new DealerLicenseData(lines[0])
            {
                businessName = new List<string>(),
                businessNumber = new List<string>(),
                line1Position = lines[7] switch
                {
                    " " => DealerLicenseData.DealerLicensePosition.None,
                    "C1" => DealerLicenseData.DealerLicensePosition.CirclePos1,
                    "C2" => DealerLicenseData.DealerLicensePosition.CirclePos2,
                    "C3" => DealerLicenseData.DealerLicensePosition.CirclePos3,
                    _ => DealerLicenseData.DealerLicensePosition.None
                },
                line2Position = lines[8] switch
                {
                    " " => DealerLicenseData.DealerLicensePosition.None,
                    "C4" => DealerLicenseData.DealerLicensePosition.CirclePos4,
                    "C5" => DealerLicenseData.DealerLicensePosition.CirclePos5,
                    _ => DealerLicenseData.DealerLicensePosition.None
                },
                line3Position = lines[9] switch
                {
                    " " => DealerLicenseData.DealerLicensePosition.None,
                    "T1" => DealerLicenseData.DealerLicensePosition.TickPos1,
                    "T2" => DealerLicenseData.DealerLicensePosition.TickPos2,
                    "T3" => DealerLicenseData.DealerLicensePosition.TickPos3,
                    _ => DealerLicenseData.DealerLicensePosition.None
                },
                line4Position = lines[10] switch
                {
                    " " => DealerLicenseData.DealerLicensePosition.None,
                    "T4" => DealerLicenseData.DealerLicensePosition.TickPos4,
                    "T5" => DealerLicenseData.DealerLicensePosition.TickPos5,
                    "T6" => DealerLicenseData.DealerLicensePosition.TickPos6,
                    _ => DealerLicenseData.DealerLicensePosition.None
                },
                isProcess = lines[11].Equals("Yes"),
                isTick = lines[12].Equals("Yes"),
                stampSign = new List<string>()
            };

            for (int i = 1; i <= 2; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.businessName.Add(lines[i]);    
            }
            
            for (int i = 3; i <= 4; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.businessNumber.Add(lines[i]);    
            }
            
            for (int i = 11; i <= 12; i++)
            {
                if(string.IsNullOrEmpty(lines[i]))
                    continue;
                data.stampSign.Add(lines[i]);    
            }
            
            dealerLicenseData.Add(data);
        }
    }

    [System.Serializable]
    public class DealerLicenseData
    {
        public enum DealerLicensePosition
        {
            None,
            CirclePos1,
            CirclePos2,
            CirclePos3,
            CirclePos4,
            CirclePos5,
            TickPos1,
            TickPos2,
            TickPos3,
            TickPos4,
            TickPos5,
            TickPos6
        }

        public string animalId;

        [Header("Business Name")]
        public List<string> businessName;

        [Header("Business Number")]
        public List<string> businessNumber;

        [Header("Line 1")]
        public DealerLicensePosition line1Position;

        [Header("Line 2")]
        public DealerLicensePosition line2Position;

        [Header("Line 3")]
        public DealerLicensePosition line3Position;

        [Header("Line 4")]
        public DealerLicensePosition line4Position;

        [Header("Line 5")]
        public bool isProcess;

        [Header("Line 6")]
        public bool isTick;
        
        [Header("Stamp & Sign Position")]
        public List<string> stampSign;

        public DealerLicenseData() { }

        public DealerLicenseData(string id)
        {
            animalId = id;
        }
    }
}
