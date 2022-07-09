using System;
using System.Collections;
using System.IO;
using Game.Scripts.Controller;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

namespace Game.Scripts.Level_State
{
    public class CutSceneState : GameCore
    {
        private CutsceneController _cutsceneController;
        private IEnumerator _loadCutsceneRoutine;
        private string[] _videoList;
        private int _currentVideoIndex;
        
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
            _currentVideoIndex++;
            if(_videoList.Length > _currentVideoIndex)
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
            var video = Addressables.LoadAssetAsync<TextAsset>(addressPath);
            yield return null;

            video.Completed += data =>
            {
                if (data.Status is AsyncOperationStatus.Failed or AsyncOperationStatus.None)
                {
                    Debug.LogError("Video not found");
                    return;
                }

                var textAsset = video.Result;
                if (!Directory.Exists(Application.persistentDataPath + "/Video Data/"))
                    Directory.CreateDirectory(Application.persistentDataPath + "/Video Data/");
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, addressPath + ".mp4"), textAsset.bytes);
                
                string url = Application.persistentDataPath + "/" + addressPath + ".mp4";
                Debug.Log(url);
                _cutsceneController.GetComponent<VideoPlayer>().url = url;
                Addressables.Release(video);
            };
            yield return null;
            
            _cutsceneController.PlayCutScene();
        }
    }
}
