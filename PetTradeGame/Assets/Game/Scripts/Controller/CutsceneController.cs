using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller
{
    public class CutsceneController : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        private Canvas canvas;

        private IEnumerator routine;
        
        public static event EventHandler CompleteEvent;
    
        // Start is called before the first frame update
        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            canvas = GetComponentInChildren<Canvas>();
            
            videoPlayer.playOnAwake = false;
            videoPlayer.targetCameraAlpha = 1f;

            canvas.gameObject.SetActive(false);
        }
        
        public void playCutScene(VideoClip video)
        {
            videoPlayer.clip = video;
            if(routine != null)
                StopCoroutine(routine);
            
            canvas.gameObject.SetActive(true);
            routine = Sequence(videoPlayer);
            StartCoroutine(routine);
        }

        private IEnumerator Sequence(VideoPlayer vp)
        {
            if (videoPlayer.clip == null)
            {
                yield return null;
                canvas.gameObject.SetActive(false);
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
                
                canvas.gameObject.SetActive(false);
                CompleteEvent?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
