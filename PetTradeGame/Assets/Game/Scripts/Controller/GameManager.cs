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
        public LevelData LevelData { get; private set; }
        public int LevelCount { get; set; }
        
        public GameObject world;
        public bool debugMode;
        public bool stopTimer;

        private IEnumerator _changeLevelRoutine;
        private AsyncOperationHandle<LevelData> _levelDataHandle;

        private void Start()
        {
            _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_Day1");
            _levelDataHandle.Completed += OnLoadLevelDataCompleted;
            LevelCount = 1;
            
            ChangeState<InitControllerState>();
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
            Addressables.Release(_levelDataHandle);
            yield return null;

            _levelDataHandle = Addressables.LoadAssetAsync<LevelData>("Level Data/LevelData_" + levelName);
            if (_levelDataHandle.OperationException is InvalidKeyException invalidKeyException)
            {
                Debug.Log($"No level data found : Go back to main menu. Exception: {invalidKeyException}");
                StopCoroutine(_changeLevelRoutine);
                LevelCount = 1;
                ChangeState<MainMenuState>();
            }
            
            _levelDataHandle.Completed += OnLoadLevelDataCompleted;
            yield return _levelDataHandle;
            ChangeState<CutSceneState>();
            StopCoroutine(_changeLevelRoutine);
        }
        
        public void ChangeLevel(string levelName)
        {
            _changeLevelRoutine = ChangeLevelData(levelName);
            StartCoroutine(_changeLevelRoutine);
        }
    }
}
