using System;
using System.Collections;
using System.IO;
using Game.Scripts.Common.State_Machine;
using Game.Scripts.Level_State;
using Game.Scripts.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.Controller
{
    public class GameManager : StateMachine
    {
        public GameObject world;
        public ConversationData globalDialogue;
        
        public bool debugMode;
        public bool stopTimer;

        private IEnumerator _changeLevelRoutine;
        private AsyncOperationHandle<LevelData> _levelDataHandle;
        private bool _isInit;
        
        public LevelData LevelData { get; private set; }
        public int LevelCount { get; set; }

        private void Start()
        {
            Application.targetFrameRate = 300;
            LevelCount = 1;
            //LoadFirstLevelData();
            ChangeLevel("Day1");
        }
        
        private void OnLoadLevelDataCompleted(AsyncOperationHandle<LevelData> data)
        {
            switch (data.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    LevelData = data.Result;
                    Debug.Log(LevelData);
                    //Addressables.Release(data);
                    break;
                case AsyncOperationStatus.Failed:
                    break;
                case AsyncOperationStatus.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator ChangeLevelData(string levelName)
        {
            if (_levelDataHandle.IsValid())
                Addressables.Release(_levelDataHandle);

            _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_" + levelName);
            if (_levelDataHandle.OperationException is InvalidKeyException invalidKeyException)
            {
                Debug.LogError($"No level data found : Go back to main menu and reload Day 1 Data.\n Exception: {invalidKeyException}");
                LevelCount = 1;
                _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_Day1");
            }

            _levelDataHandle.Completed += OnLoadLevelDataCompleted;
            yield return _levelDataHandle;

            if (!_isInit)
            {
                if (debugMode)
                {
                    GamePlayController.IsDebugMode = true;
                    UIController.IsDebugMode = true;
                }

                _isInit = true;
                ChangeState<InitState>();
            }
            else
            {
                if (debugMode)
                    ChangeState<DialogueState>();
                else
                    ChangeState<CutSceneState>();
            }
            
            StopCoroutine(_changeLevelRoutine);
        }
        
        public void ChangeLevel(string levelName)
        {
            _changeLevelRoutine = ChangeLevelData(levelName);
            StartCoroutine(_changeLevelRoutine);
        }

        public void ClearGameData()
        {
            Debug.Log("Start Clearing Data");
            var path = Application.persistentDataPath + "/Video Data/";
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                Directory.Delete(path);
            }

            path = Application.persistentDataPath + "/Save Data/";
            if (Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }
            }
            
            Debug.Log("Clearing Data Complete");
        }
    }
}