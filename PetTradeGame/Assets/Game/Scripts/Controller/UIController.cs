using System;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        public static bool IsDebugMode = false;

        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private GamePlayPanel gamePlayPanel;
        [SerializeField] private EndGamePanel endGamePanel;

        private Canvas _canvas;
        
        public static event EventHandler StartGameEvent;
        public static event EventHandler NextDayEvent;

        private void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.enabled = true;
        }

        public void Init()
        {
            if(IsDebugMode)
            {
                gamePlayPanel.gameObject.SetActive(true);
                mainMenuPanel.gameObject.SetActive(false);
            }
            else
                mainMenuPanel.gameObject.SetActive(true);
        }

        #region UI Behavior
        public void SetScore(int cs, int correctDoc, int wrongDoc)
        {
            endGamePanel.scoreText.text = cs.ToString();
            endGamePanel.correctDocText.text = correctDoc.ToString();
            endGamePanel.wrongDocText.text = wrongDoc.ToString();
        }

        public void StartLevel()
        {
            gamePlayPanel.gameObject.SetActive(true);
        }

        public void EndLevel()
        {
            gamePlayPanel.gameObject.SetActive(false);
            endGamePanel.gameObject.SetActive(true);
        }
        
        public void OnBtnStartClicked()
        {
            //gamePlayPanel.gameObject.SetActive(true);
            StartGameEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnBtnNextLevelClicked()
        {
            NextDayEvent?.Invoke(this, EventArgs.Empty);
        }

        //TODO: Setting UI
        /*public void OnBtnSettingClicked()
        {
            
        }

        public void OnBtnCancelClicked()
        {
            
        }*/
        
        #endregion
    }
}
