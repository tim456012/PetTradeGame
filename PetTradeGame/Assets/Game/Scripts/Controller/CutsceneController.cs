using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller
{
    public class CutsceneController : MonoBehaviour
    {
        //private AudioSource _audioSource;
        private Canvas _canvas;
        private MonoRoutine _routine;
        private VideoPlayer _videoPlayer;

        // Start is called before the first frame update
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

            //if(!_routine.IsRunning)
            _routine.Start();
        }

        private IEnumerator PlayVideo()
        {
            VideoPlayer vp = _videoPlayer;
            vp.Prepare();

            //var waitTime = new WaitForSeconds(5f);
            while (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                yield return null;
                //yield return waitTime;
                //break;
            }

            Debug.Log($"Status: {vp.isPrepared}, Video is prepared");
            vp.Play();
            //_audioSource.Play();

            Debug.Log("Video is playing");
            while (vp.isPlaying)
            {
                Debug.Log($"Video Time : {Mathf.FloorToInt((float)vp.time)}");
                yield return null;
            }

            Debug.Log("Video is finished");

            vp.errorReceived += (player, message) =>
            {
                Debug.Log($"Error received : {message}");
            };
        }

        private void OnRoutineStarted(object sender, EventArgs e)
        {
            if (_videoPlayer.url != null)
                return;

            _canvas.gameObject.SetActive(false);
            _routine.Stop();
        }

        private void OnRoutineFinished(object sender, EventArgs e)
        {
            _videoPlayer.Stop();
            _videoPlayer.enabled = false;

            _canvas.gameObject.SetActive(false);
            _routine.Stop();
            CompleteEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}