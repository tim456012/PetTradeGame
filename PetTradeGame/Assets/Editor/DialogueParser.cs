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
            if (!AssetDatabase.IsValidFolder("Assets/Game/Data/Conversations"))
                AssetDatabase.CreateFolder("Assets/Game/Data", "Conversations");
        }

        private static void ParseAllDialogueData()
        {
            var readPath = $"{Application.dataPath}/Game/Settings/Dialogue";
            const string targetPath = "Assets/Game/Data/Conversations/";
            Debug.Log(readPath);

            var directoryInfo = new DirectoryInfo(readPath);
            var fileInfos = directoryInfo.GetFiles("*.csv");

            foreach (FileInfo file in fileInfos)
            {
                Debug.Log(file.Name);
                if (!File.Exists(file.ToString()))
                {
                    Debug.LogError($"Missing Dialogue Data : {file}");
                    return;
                }

                var readText = File.ReadAllLines(file.ToString());
                var conversationData = ScriptableObject.CreateInstance<ConversationData>();
                for (var i = 1; i < readText.Length; ++i)
                {
                    conversationData.Load(readText[i]);
                }
                var assetName = file.Name;
                var filExtPos = assetName.LastIndexOf('.');
                if (filExtPos >= 0)
                {
                    assetName = assetName[..filExtPos];
                }

                var fileName = $"{targetPath}{assetName}.asset";
                AssetDatabase.CreateAsset(conversationData, fileName);
            }
        }
    }
}