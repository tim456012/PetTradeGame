using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Model
{
    [System.Serializable]
    public class FunctionalObjectsData
    {
        public AssetReference prefab;
        public string key;
        public int amount;
        public int maxAmount;
        public string spawnPosition;
    }
}
