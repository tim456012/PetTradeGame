using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [System.Serializable]
    public class ScoreData
    {
        public string id;
        [Range(1, 10)]
        public int score;
        public bool isWrongDocument;
    }
}
