using System;
using System.Collections;
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
        public bool debugMode;
        public bool stopTimer;

        private IEnumerator _changeLevelRoutine;
        private AsyncOperationHandle<LevelData> _levelDataHandle;
        public LevelData LevelData { get; private set; }
        public int LevelCount { get; set; }

        private void Start()
        {
            LevelCount = 1;
            LoadFirstLevelData();
            ChangeState<InitState>();
        }
        
        private void OnLoadLevelDataCompleted(AsyncOperationHandle<LevelData> data)
        {
            switch (data.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    LevelData = data.Result;
                    Debug.Log(LevelData);
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
            yield return null;

            _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_" + levelName);
            if (_levelDataHandle.OperationException is InvalidKeyException invalidKeyException)
            {
                Debug.LogError($"No level data found : Go back to main menu and reload Day 1 Data.\n Exception: {invalidKeyException}");
                LevelCount = 1;
                _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_Day1");
            }

            _levelDataHandle.Completed += OnLoadLevelDataCompleted;
            yield return _levelDataHandle;

            if (GetState<InitState>() && debugMode)
            {
                GamePlayController.IsDebugMode = true;
                UIController.IsDebugMode = true;
            }

            if (debugMode)
                ChangeState<DialogueState>();
            else
                ChangeState<MainMenuState>();

            StopCoroutine(_changeLevelRoutine);
            _levelDataHandle.Completed -= OnLoadLevelDataCompleted;
        }
        
        private void LoadFirstLevelData()
        {
            var levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_Day1");
            if (levelDataHandle.OperationException is InvalidKeyException invalidKeyException)
                Debug.LogError($"No level data found. \n Exception: {invalidKeyException}");

            levelDataHandle.Completed += OnLoadLevelDataCompleted;
            Addressables.Release(levelDataHandle);
        }
        
        public void ChangeLevel(string levelName)
        {
            _changeLevelRoutine = ChangeLevelData(levelName);
            StartCoroutine(_changeLevelRoutine);
        }
    }
}