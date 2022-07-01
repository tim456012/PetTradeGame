using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        public VideoClip video1;
        public VideoClip video2;
        public VideoClip video3;
        public ConversationData introDialogue;
        public ConversationData middleDialogue;
        public ConversationData outroDialogue;
        public ConversationData otherDialogue;
        public ScoreData scoreData;
        public List<RecipeData> documentRecipeData;
        public List<FunctionalObjectsData> functionalObjectsData;
    }
}
