using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.Model;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Dealer License Recipe", menuName = "ScriptableObject/Dealer License Recipe")]
    public class DealerLicenseRecipe : ScriptableObject
    {
        public List<DealerLicenseData> dealerLicenseData;
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
    }
}
