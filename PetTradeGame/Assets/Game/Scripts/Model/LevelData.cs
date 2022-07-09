using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Videos")]
        public string[] videoAddress;
        
        [Header("Dialogue")]
        public AssetReference introDialogue;
        public AssetReference middleDialogue;
        
        [Header("Data")]
        public List<RecipeData> documentRecipeData;
        public List<FunctionalObjectsData> functionalObjectsData;
        public List<ScoreData> scoreData;
    }
}
