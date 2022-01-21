using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SettingsAutoConverter : AssetPostprocessor
{
    private static Dictionary<string, Action> _parsers;

    static SettingsAutoConverter()
    {
        _parsers = new Dictionary<string, Action>();
        _parsers.Add("Enemies.csv", ParseEnemies);
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

    private static void ParseEnemies()
    {
        string filePath = Application.dataPath + "/Game/Settings/Enemies.csv";
        Debug.Log(filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Missing Enemies Data : {filePath}");
            return;
        }

        string[] readText = File.ReadAllLines("Assets/Game/Settings/Enemies.csv");
        filePath = "Assets/Resources/";
        for (int i = 1; i < readText.Length; ++i)
        {
            EnemyData enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.Load(readText[i]);
            string fileName = $"{filePath}{enemyData.name}.asset";
            AssetDatabase.CreateAsset(enemyData, fileName);
        }
    }
}