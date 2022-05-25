using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        public VideoClip firstVideo;
        public VideoClip secondVideo;
        public ConversationData introDialogue;
        public ConversationData middleDialogue;
        public ConversationData outroDialogue;
        public ConversationData otherDialogue;

        public List<string> documentRecipeName;
        public List<FunctionalObjectsData> functionalObjectsData;
    }
}
