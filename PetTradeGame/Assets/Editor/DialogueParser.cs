using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Assets.Game.Scripts.Enum;
using Assets.Game.Scripts.Model;
using UnityEngine;
using UnityEditor;

public static class DialogueParser
{
    private static readonly Regex Regex = new("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);

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
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Conversations");
        }
    }

    private static void ParseAllDialogueData()
    {
        string readPath = $"{Application.dataPath}/Game/Settings/Dialogue";
        const string targetPath = "Assets/Resources/Conversations/";
        Debug.Log(readPath);

        DirectoryInfo directoryInfo = new DirectoryInfo(readPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.csv");

        foreach (FileInfo file in fileInfos)
        {
            Debug.Log(file.Name);
            if (!File.Exists(file.ToString()))
            {
                Debug.LogError($"Missing Dialogue Data : {file}");
                return;
            }

            string[] readText = File.ReadAllLines(file.ToString());
            ConversationData conversationData = ScriptableObject.CreateInstance<ConversationData>();
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
