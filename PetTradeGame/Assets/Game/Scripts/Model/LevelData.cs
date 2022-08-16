using System;
using System.Collections.Generic;
using Game.Scripts.Enum;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Videos")]
        public string[] videoAddress;

        [Header("Dialogue")]
        public AssetReference dialogue;

        [Header("Data")]
        public List<RecipeData> documentRecipeData;
        public List<FunctionalObjectsData> functionalObjectsData;
        public List<ScoreData> scoreData;
        public List<Sprite> animalGuide;
    }

    [Serializable]
    public class FunctionalObjectsData
    {
        public AssetReference prefab;
        public string key;
        public string spawnPosition;
    }

    [Serializable]
    public class RecipeData
    {
        public DocumentType documentRecipeType;
        public AssetReference documentRecipe;
        //public string documentRecipeName;
    }

    [Serializable]
    public class ScoreData
    {
        public string id;
        [Range(1, 10)]
        public int score;
        public bool isWrongDocument;
    }
}