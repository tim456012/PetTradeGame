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

        public static event EventHandler CompleteEvent;
    
        // Start is called before the first frame update
        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            canvas = GetComponentInChildren<Canvas>();
            
            videoPlayer.playOnAwake = false;
            videoPlayer.targetCameraAlpha = 1f;
        }

        public void playCutScene()
        {
            canvas.gameObject.SetActive(true);
            StartCoroutine(Sequence(videoPlayer));
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
            
            if (!vp.isPrepared)
            {
                vp.Prepare();
                yield return null;
            }
            else
                vp.Play();
            
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
