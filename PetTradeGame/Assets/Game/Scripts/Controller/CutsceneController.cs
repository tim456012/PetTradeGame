using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.Scripts.Controller
{
    public class CutsceneController : MonoBehaviour
    {
        public Button btnSkipVideo;
        
        //private AudioSource _audioSource;
        private Canvas _canvas;
        private MonoRoutine _routine;
        private VideoPlayer _videoPlayer;
        
        private void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _canvas = GetComponentInChildren<Canvas>();

            _videoPlayer.playOnAwake = true;
            //_audioSource.playOnAwake = false;

            _videoPlayer.targetCameraAlpha = 1f;
            //_videoPlayer.SetTargetAudioSource(0, _audioSource);
            _videoPlayer.Stop();

            _canvas.gameObject.SetActive(false);

            _routine = new MonoRoutine(PlayVideo, this);
            _routine.Started += OnRoutineStarted;
            _routine.Stopped += OnRoutineFinished;
        }

        private void OnDisable()
        {
            _routine.Started -= OnRoutineStarted;
            _routine.Stopped -= OnRoutineFinished;
        }

        public static event EventHandler CompleteEvent;

        public void PlayCutScene()
        {
            _videoPlayer.enabled = true;
            _canvas.gameObject.SetActive(true);

            if(!_routine.IsRunning)
                _routine.Start();
        }

        private IEnumerator PlayVideo()
        {
            VideoPlayer vp = _videoPlayer;
            vp.Prepare();

            while (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                yield return null;
            }

            Debug.Log($"Status: {vp.isPrepared}, Video is prepared");
            vp.Play();

            Debug.Log("Video is playing");
            while (vp.isPlaying)
            {
                Debug.Log($"Video Time : {Mathf.FloorToInt((float)vp.time)}");
                yield return null;
                
                btnSkipVideo.onClick.AddListener(delegate
                {
                    _routine.Stop();
                });
            }

            Debug.Log("Video is finished");
            vp.errorReceived += (player, message) =>
            {
                Debug.Log($"Error received : {message}");
            };
        }

        private void OnRoutineStarted(object sender, EventArgs e)
        {
            btnSkipVideo.gameObject.SetActive(true);
            if (_videoPlayer.url != null)
                return;

            _canvas.gameObject.SetActive(false);
            _routine.Stop();
        }

        private void OnRoutineFinished(object sender, EventArgs e)
        {
            _videoPlayer.Stop();
            _videoPlayer.enabled = false;
            
            btnSkipVideo.gameObject.SetActive(false);
            _canvas.gameObject.SetActive(false);
            _routine.Stop();
            CompleteEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}