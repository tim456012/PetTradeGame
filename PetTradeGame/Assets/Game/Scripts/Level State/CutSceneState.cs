using System;
using System.Collections;
using Game.Scripts.Controller;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

namespace Game.Scripts.Level_State
{
    public class CutSceneState : GameCore
    {
        private int _currentVideoIndex;
        private CutsceneController _cutsceneController;
        private IEnumerator _loadCutsceneRoutine;
        private AsyncOperationHandle _videoHandle;
        private string[] _videoList;

        protected override void Awake()
        {
            base.Awake();
            _cutsceneController = Owner.GetComponentInChildren<CutsceneController>();
            _currentVideoIndex = 0;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _videoList = null;
            _currentVideoIndex = 0;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log(Owner.LevelData.name);
            _videoList = new string[Owner.LevelData.videoAddress.Length];
            _videoList = Owner.LevelData.videoAddress;

            //Load first video
            LoadVideo(_videoList[0]);
        }

        public override void Exit()
        {
            base.Exit();
            _videoList = null;
            _currentVideoIndex = 0;
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            CutsceneController.CompleteEvent += OnCompleteVideoPlaying;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            CutsceneController.CompleteEvent -= OnCompleteVideoPlaying;
        }

        private void LoadVideo(string path)
        {
            _loadCutsceneRoutine = LoadVideoData(path);
            StartCoroutine(_loadCutsceneRoutine);
        }

        private void OnCompleteVideoPlaying(object sender, EventArgs e)
        {
            Addressables.Release(_videoHandle);
            _currentVideoIndex++;
            if (_videoList.Length > _currentVideoIndex)
            {
                if (!string.IsNullOrEmpty(_videoList[_currentVideoIndex]))
                {
                    //Reload video
                    LoadVideo(_videoList[_currentVideoIndex]);
                    return;
                }
            }

            _videoList = null;
            Owner.ChangeState<DialogueState>();
        }
        
        private IEnumerator LoadVideoData(string addressPath)
        {
            var path = $"Assets/Game/Textures/Videos/{addressPath}.mp4";
            Debug.Log(path);
            
            var handle = path.Get<VideoClip>();
            if (!handle.IsDone)
                yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Video not found");
                Addressables.Release(handle);
                yield break;
            }

            VideoClip video = handle.Result;
            _videoHandle = handle;
            _cutsceneController.GetComponent<VideoPlayer>().clip = video;
            yield return null;
            
            _cutsceneController.PlayCutScene();
        }
    }
}