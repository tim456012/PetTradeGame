using System;
using System.Collections.Generic;
using System.IO;
using Assets.Game.Scripts.Model;
using UnityEditor;
using UnityEngine;

public class SettingsAutoConverter : AssetPostprocessor
{
    private static readonly Dictionary<string, Action> _parsers;

    static SettingsAutoConverter()
    {
        _parsers = new Dictionary<string, Action>();
        _parsers.Add("Dialogue Test.csv", ParseDialogues);
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
        string filePath = Application.dataPath + "/Game/Settings/Dialogue Test.csv";
        Debug.Log(filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Missing Dialogue Data : {filePath}");
            return;
        }

        string[] readText = File.ReadAllLines("Assets/Game/Settings/Dialogue Test.csv");
        filePath = "Assets/Resources/";
        for (int i = 1; i < readText.Length; ++i)
        {
            ConversationData dialogueData = ScriptableObject.CreateInstance<ConversationData>();
            dialogueData.Load(readText[i]);
            string fileName = $"{filePath}{dialogueData.name}.asset";
            AssetDatabase.CreateAsset(dialogueData, fileName);
        }
    }
}