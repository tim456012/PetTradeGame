using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Special Permit Recipe", menuName = "ScriptableObject/Special Permit Recipe")]
    public class SpecialPermitRecipe : ScriptableObject
    {
        public List<SpecialPermitInformation> specialPermitData;

    }

    [System.Serializable]
    public class SpecialPermitInformation
    {
        public enum SpecialPermitPosition
        {
            CirclePos1,
            CirclePos2,
            CirclePos3,
            CirclePos4,
            CirclePos5,
            CirclePos6
        }

        public string animalId;

        [Header("Location Name")]
        public List<string> locationName;

        [Header("Line 1")]
        public SpecialPermitPosition line1Position;

        [Header("Line 2")]
        public SpecialPermitPosition line2Position;
        
        [Header("Business Number")]
        public List<string> businessName;

        [Header("Deadline")]
        public List<string> deadline;

        [Header("Animal Name")]
        public List<string> animalName;

        [Header("Animal Count")]
        public List<string> animalCount;

        [Header("Animal Feature")]
        public List<string> animalFeature;
        
        [Header("Stamp & Sign Position")]
        public List<GameObject> prefab;
    }
}
