using System.IO;
using Game.Scripts.Model;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class DialogueParser
    {
        [MenuItem("Pre Production/Parse Dialogue Data")]
        public static void Parse()
        {
            Initialize();
            ParseAllDialogueData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void Initialize()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Conversations"))
                AssetDatabase.CreateFolder("Assets/Resources", "Conversations");
        }

        private static void ParseAllDialogueData()
        {
            string readPath = $"{Application.dataPath}/Game/Settings/Dialogue";
            const string targetPath = "Assets/Resources/Conversations/";
            Debug.Log(readPath);

            var directoryInfo = new DirectoryInfo(readPath);
            var fileInfos = directoryInfo.GetFiles("*.csv");

            foreach (var file in fileInfos)
            {
                Debug.Log(file.Name);
                if (!File.Exists(file.ToString()))
                {
                    Debug.LogError($"Missing Dialogue Data : {file}");
                    return;
                }

                string[] readText = File.ReadAllLines(file.ToString());
                var conversationData = ScriptableObject.CreateInstance<ConversationData>();
                for (int i = 1; i < readText.Length; ++i)
                {
                    conversationData.Load(readText[i]);
                }
                string assetName = file.Name;
                int filExtPos = assetName.LastIndexOf('.');
                if (filExtPos >= 0)
                {
                    assetName = assetName[..filExtPos];
                }

                string fileName = $"{targetPath}{assetName}.asset";
                AssetDatabase.CreateAsset(conversationData, fileName);
            }
        }
    }
}
