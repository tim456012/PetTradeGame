using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller
{
    public class CutsceneController : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;
        private Canvas _canvas;
        private MonoRoutine _routine;

        public static event EventHandler CompleteEvent;

        // Start is called before the first frame update
        private void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _canvas = GetComponentInChildren<Canvas>();

            _videoPlayer.playOnAwake = false;
            _videoPlayer.targetCameraAlpha = 1f;

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

        public void PlayCutScene(VideoClip video)
        {
            _videoPlayer.enabled = true;
            _videoPlayer.clip = video;
            Debug.Log($"{_videoPlayer.clip}");
            _canvas.gameObject.SetActive(true);
            
            //if(!_routine.IsRunning)
                _routine.Start();
        }

        private IEnumerator PlayVideo()
        {
            var vp = _videoPlayer;
            vp.Prepare();

            while (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                //yield return null;
            }
            
            vp.prepareCompleted += player =>
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is prepared");
                player.Play();
            };
            
            while (vp.isPlaying)
            {                
                Debug.Log("Video is playing");
                yield return null;
            }

            vp.errorReceived += (player, message) =>
            {
                Debug.Log($"Error received : {message}");
            };

            vp.loopPointReached += player =>
            {
                Debug.Log("Video is end.");
                player.clip = null;
                player.enabled = false;
            };
        }

        private void OnRoutineStarted(object sender, EventArgs e)
        {
            //if (_videoPlayer.clip != null)
            //    return;
            
            //_canvas.gameObject.SetActive(false);
            //_routine.Stop();
        }
        
        private void OnRoutineFinished(object sender, EventArgs e)
        {
            //_canvas.gameObject.SetActive(false);
            //_routine.Stop();
            //CompleteEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
