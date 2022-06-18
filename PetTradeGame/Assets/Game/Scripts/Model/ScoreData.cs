using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "ScoreData", menuName = "ScriptableObject/Score Data")]
    public class ScoreData : ScriptableObject
    {
        public List<ScoreContent> scoreContents;
    }

    [System.Serializable]
    public class ScoreContent
    {
        public string id;
        [Range(-100, 100)]
        public int score;
    }
}
