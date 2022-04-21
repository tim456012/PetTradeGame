using System;
using System.Buffers;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Scripts.Controller
{
    public class CutsceneController : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        private Canvas canvas;

        private Coroutine routine;
        
        public static event EventHandler CompleteEvent;
    
        // Start is called before the first frame update
        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            canvas = GetComponentInChildren<Canvas>();
            

            videoPlayer.playOnAwake = false;
            videoPlayer.targetCameraAlpha = 1f;
        }
        
        public void playCutScene(VideoClip video)
        {
            videoPlayer.clip = video;
            if(routine != null)
                StopCoroutine(routine);
            
            canvas.gameObject.SetActive(true);
            routine = StartCoroutine(Sequence(videoPlayer));
        }

        private IEnumerator Sequence(VideoPlayer vp)
        {
            if (vp.clip == null)
            {
                Debug.Log("Video is null");
                yield return null;
                canvas.gameObject.SetActive(false);
                CompleteEvent?.Invoke(this, EventArgs.Empty);
                yield break;
            }
            
            while (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                vp.Prepare();
            }
            
            
            if (!vp.isPrepared)
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is not prepare");
                vp.Prepare();
                yield return new WaitForSecondsRealtime(3);
                //yield return null;
            }
            else
            {
                Debug.Log($"Status: {vp.isPrepared}, Video is prepared");
                vp.Play();
            }

            vp.errorReceived += (source, message) => 
            {
                Debug.Log($"Error received : {message}");
            };
            
            if(vp.isPlaying)
                Debug.Log("Video is playing");
            
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
