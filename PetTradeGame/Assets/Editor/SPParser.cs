using System.IO;
using Game.Scripts.Factory;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SPParser
    {
        [MenuItem("Pre Production/Parse Special Permit Data")]
        public static void Parse()
        {
            Initialize();
            ParseData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void Initialize()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Game/Data/Recipes"))
                AssetDatabase.CreateFolder("Assets/Game/Data", "Recipes");
        }

        private static void ParseData()
        {
            string readPath = $"{Application.dataPath}/Game/Settings/Recipes/Special Permit";
            const string targetPath = "Assets/Game/Data/Recipes/Special Permit/";
            Debug.Log(readPath);

            var directoryInfo = new DirectoryInfo(readPath);
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
                var data = ScriptableObject.CreateInstance<SpecialPermitRecipe>();
                for (int i = 1; i < readText.Length; ++i)
                {
                    data.Load(readText[i]);
                }
                string assetName = file.Name;
                int filExtPos = assetName.LastIndexOf('.');
                if (filExtPos >= 0)
                {
                    assetName = assetName[..filExtPos];
                }

                string fileName = $"{targetPath}{assetName}.asset";
                AssetDatabase.CreateAsset(data, fileName);
            }
        }
    }
}