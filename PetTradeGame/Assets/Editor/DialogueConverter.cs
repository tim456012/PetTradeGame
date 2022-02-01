using System;
using System.Collections.Generic;
using System.IO;
using Assets.Game.Scripts.Model;
using UnityEditor;
using UnityEngine;

public class DialogueConverter : AssetPostprocessor
{
    private static readonly Dictionary<string, Action> _parsers;
    private static int count = 1;

    static DialogueConverter()
    {
        _parsers = new Dictionary<string, Action>
        {
            {
                "Day1.csv", ParseDialogues
            },
            {
                "Day2.csv", ParseDialogues
            }
        };
    }

    private static void OnPostprocessAllAssets(string[] importedAssets,
        string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var name in importedAssets)
        {
            string fileName = Path.GetFileName(name);
            if (_parsers.ContainsKey(fileName))
            {
                _parsers[fileName]();
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ParseDialogues()
    {
        string filePath = Application.dataPath + $"/Game/Settings/Day{count}.csv";
        Debug.Log(filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Missing Dialogue Data : {filePath}");
            return;
        }

        string[] readText = File.ReadAllLines($"Assets/Game/Settings/Day{count}.csv");
        filePath = "Assets/Resources/";
        ConversationData conversationData = ScriptableObject.CreateInstance<ConversationData>();
        for (int i = 1; i < readText.Length; ++i)
        {
            conversationData.Load(readText[i]);
        }
        string fileName = $"{filePath}Day{count}.asset";
        AssetDatabase.CreateAsset(conversationData, fileName);
        count++;
    }
}