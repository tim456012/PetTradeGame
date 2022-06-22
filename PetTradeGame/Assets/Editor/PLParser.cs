using System.IO;
using Game.Scripts.Factory;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PLParser
    {
        [MenuItem("Pre Production/Parse Possession License Data")]
        public static void Parse()
        {
            Initialize();
            ParseData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static void Initialize()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Recipes"))
                AssetDatabase.CreateFolder("Assets/Resources", "Recipes");
        }

        private static void ParseData()
        {
            string readPath = $"{Application.dataPath}/Game/Settings/Recipes/Possession License";
            const string targetPath = "Assets/Resources/Recipes/Possession License/";
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
                var data = ScriptableObject.CreateInstance<PossessionLicenseRecipe>();
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