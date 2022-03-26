using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObject/Level Data")]
    public class LevelData : ScriptableObject
    {
        public ConversationData CutSceneDialogue;
        public ConversationData IntroDialogue;
        public ConversationData MiddleDialogue;
        public ConversationData OutroDialogue;
        public ConversationData OtherDialogue;

        public List<string> DocumentsNeeded;
        public List<FunctionalObjectsData> FunctionalObjectsData;
    }
}
