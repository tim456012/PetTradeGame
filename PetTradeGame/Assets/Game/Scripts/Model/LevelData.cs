using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        public VideoClip cutSceneVideo;
        public ConversationData introDialogue;
        public ConversationData middleDialogue;
        public ConversationData outroDialogue;
        public ConversationData otherDialogue;

        public List<string> DocumentsNeeded;
        public List<FunctionalObjectsData> FunctionalObjectsData;
    }
}
