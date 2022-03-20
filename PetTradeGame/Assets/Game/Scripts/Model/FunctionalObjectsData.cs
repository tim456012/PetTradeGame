using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class FunctionalObjectsData
    {
        public GameObject Object;
        public string Key;
        public int Amount;
        public int MaxAmount;
        public string SpawnPosition;
    }
}