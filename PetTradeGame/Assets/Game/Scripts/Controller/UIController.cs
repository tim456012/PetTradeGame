using System;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;

        [SerializeField] private MainMenuPanel mainMenuPanel;
        
        private Canvas canvas;
        
        public static event EventHandler StartGameEvent;
        
        //private Tweener transition;
        
        // Start is called before the first frame update
        private void Start()
        {
            canvas = GetComponentInChildren<Canvas>();

            mainMenuPanel.btnStart.onClick.AddListener(OnBtnStartClicked);
        }

        public void setScore(int s)
        {
            score.text = $"Score : {s}";
        }

        private void OnBtnStartClicked()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
