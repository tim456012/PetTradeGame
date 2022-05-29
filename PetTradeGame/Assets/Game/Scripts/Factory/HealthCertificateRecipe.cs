using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Health Certificate Recipe", menuName = "ScriptableObject/Health Certificate Recipe")]
    public class HealthCertificateRecipe : ScriptableObject
    { 
        public List<HealthCertificateData> healthCertificateData;
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
    }
}
