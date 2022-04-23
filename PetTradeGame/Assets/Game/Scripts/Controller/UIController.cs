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

        private Canvas canvas;
        
        public static event EventHandler StartGameEvent;
        
        //private Tweener transition;
        
        // Start is called before the first frame update
        private void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            mainMenuPanel.btnStart.onClick.AddListener(OnBtnStartClicked);
            gamePlayPanel.gameObject.SetActive(false);
        }

        private void OnBtnStartClicked()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);
            gamePlayPanel.gameObject.SetActive(true);
        }
        
        public void setScore(int s)
        {
            gamePlayPanel.setScore(s);
        }
    }
}
