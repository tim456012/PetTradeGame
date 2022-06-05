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
        private IEnumerator _routine;

        public static event EventHandler CompleteEvent;

        // Start is called before the first frame update
        private void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _canvas = GetComponentInChildren<Canvas>();

            _videoPlayer.playOnAwake = false;
            _videoPlayer.targetCameraAlpha = 1f;

            _canvas.gameObject.SetActive(false);
        }

        public void PlayCutScene(VideoClip introVideo)
        {
            _videoPlayer.enabled = true;
            _videoPlayer.clip = introVideo;
            if (_routine != null)
                StopCoroutine(_routine);

            _canvas.gameObject.SetActive(true);
            _routine = Sequence(_videoPlayer);
            StartCoroutine(_routine);
        }

        private IEnumerator Sequence(VideoPlayer vp)
        {
            if (_videoPlayer.clip == null)
            {
                yield return null;
                _canvas.gameObject.SetActive(false);
                CompleteEvent?.Invoke(this, EventArgs.Empty);
                yield break;
            }

            vp.Prepare();

            if (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                //vp.Prepare();
                yield return null;
            }

            vp.prepareCompleted += player =>
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is prepared");
                player.Play();
                Debug.Log("Video is playing");
            };

            vp.errorReceived += (player, message) =>
            {
                Debug.Log($"Error received : {message}");
            };

            vp.loopPointReached += player =>
            {
                Debug.Log("Video is end.");
                player.clip = null;
                player.enabled = false;
                
                
                _canvas.gameObject.SetActive(false);
                CompleteEvent?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
