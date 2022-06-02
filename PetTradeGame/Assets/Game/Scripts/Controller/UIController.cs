using System;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private GamePlayPanel gamePlayPanel;
        [SerializeField] private EndGamePanel endGamePanel;

        private Canvas _canvas;
        //private bool _isDebugMode;
        
        public static event EventHandler StartGameEvent;

        //private Tweener transition;

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            mainMenuPanel.btnStart.onClick.AddListener(OnBtnStartClicked);
        }

        private void OnBtnStartClicked()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);
            gamePlayPanel.gameObject.SetActive(true);
        }

        public void SetScore(int s)
        {
            gamePlayPanel.SetScore(s);
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
            //_isDebugMode = true;
        }

        public void EndGame()
        {
            endGamePanel.gameObject.SetActive(true);
        }
    }
}
