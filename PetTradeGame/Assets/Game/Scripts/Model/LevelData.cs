using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        public VideoClip introVideo;
        public VideoClip dayIntroVideo;
        public ConversationData introDialogue;
        public ConversationData middleDialogue;
        public ConversationData outroDialogue;
        public ConversationData otherDialogue;
        public ScoreData scoreData;
        public List<RecipeData> documentRecipeData;
        public List<FunctionalObjectsData> functionalObjectsData;
    }
}
