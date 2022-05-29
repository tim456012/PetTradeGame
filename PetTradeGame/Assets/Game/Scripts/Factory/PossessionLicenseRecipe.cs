using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Possession License Recipe", menuName = "ScriptableObject/Possession License Recipe")]
    public class PossessionLicenseRecipe : ScriptableObject
    {
        public List<PossessionLicenseData> possessionLicenseData;
    }

    [System.Serializable]
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
    }
}
