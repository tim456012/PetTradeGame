using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class MainMenuPanel : MonoBehaviour
    {
        public Button btnStart;
        public Button btnExit;

        
        private void Start()
        {
            btnStart.onClick.AddListener(OnBtnStartClicked);
            btnExit.onClick.AddListener(OnBtnExitClicked);
        }

        private void OnBtnStartClicked()
        {
            Debug.Log("Start Game");
            
            //StartGameEvent?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        private void OnBtnExitClicked()
        {
            Debug.Log("Exit Game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit(0);
            #endif
            
            GC.Collect();
        }
    }
}
