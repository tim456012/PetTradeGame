using System;
using Game.Scripts.EventArguments;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private GamePlayPanel gamePlayPanel;
        [SerializeField] private EndGamePanel endGamePanel;

        private Canvas _canvas;
        
        public static event EventHandler StartGameEvent;
        public static event EventHandler NextDayEvent;
        
        //private Tweener transition;

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.enabled = true;
            
            mainMenuPanel.gameObject.SetActive(true);
            mainMenuPanel.btnStart.onClick.AddListener(OnBtnStartClicked);
        }

        private void OnBtnStartClicked()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);
            gamePlayPanel.gameObject.SetActive(true);
        }

        private void OnBtnNextDayClicked()
        {
            NextDayEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetScore(int cs, int correctDoc, int wrongDoc)
        {
            endGamePanel.scoreText.text = cs.ToString();
            endGamePanel.correctDocText.text = correctDoc.ToString();
            endGamePanel.wrongDocText.text = wrongDoc.ToString();
        }

        public void SetTime(string text)
        {
            gamePlayPanel.SetTime(text);
        }
        
        public void SetDebugMode()
        {
            gamePlayPanel.gameObject.SetActive(true);
            mainMenuPanel.gameObject.SetActive(false);
        }

        public void EndGame()
        {
            endGamePanel.gameObject.SetActive(true);
            endGamePanel.btnNextDay.onClick.AddListener(OnBtnNextDayClicked);
        }
    }
}
