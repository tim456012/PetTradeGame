using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Score Data", menuName = "Score Data")]
    public class ScoreData : ScriptableObject
    {
        public List<ScoreContent> scoreContents;
    }

    [System.Serializable]
    public class ScoreContent
    {
        public string id;
        public bool isCorrect;
        [Range(-100, 100)]
        public int score;
    }
}
