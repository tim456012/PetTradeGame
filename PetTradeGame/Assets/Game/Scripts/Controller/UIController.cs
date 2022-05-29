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

        private Canvas _canvas;
        //private bool _isDebugMode;
        
        public static event EventHandler StartGameEvent;

        //private Tweener transition;

        // Start is called before the first frame update
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

        public void SetDebugMode()
        {
            gamePlayPanel.gameObject.SetActive(true);
            //_isDebugMode = true;
        }
    }
}
