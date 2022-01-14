using Game.Scripts.Model;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class AssetCreator
    {
        [MenuItem("Assets/Create/Conversation Data")]
        public static void CreateConversationData()
        {
            ScriptableObjectUtility.CreateAsset<ConversationData>();
        }
    }
}
