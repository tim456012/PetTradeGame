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
        [SerializeField] private IpadViewPanel ipadViewPanel;
        [SerializeField] private EndGamePanel endGamePanel;

        private Canvas _canvas;

        public static event EventHandler StartGameEvent;
        public static event EventHandler NextDayEvent;
        
        private void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.enabled = true;
            enabled = false;
        }
        
        public void Init()
        {
            enabled = true;
            if (IsDebugMode)
            {
                gamePlayPanel.gameObject.SetActive(true);
                mainMenuPanel.gameObject.SetActive(false);
            }
            else
            {
                gamePlayPanel.gameObject.SetActive(false);
                mainMenuPanel.gameObject.SetActive(true);
            }
        }

        #region UI Behavior

        public void SetScore(int cs, int correctDoc, int wrongDoc)
        {
            endGamePanel.scoreText.text = cs.ToString();
            endGamePanel.correctDocText.text = correctDoc.ToString();
            endGamePanel.wrongDocText.text = wrongDoc.ToString();
        }

        public void ShowGameplayPanel()
        {
            gamePlayPanel.gameObject.SetActive(true);
        }

        public void ShowEndGamePanel()
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
        
        public void ShowIpadViewPanel()
        {
            ipadViewPanel.gameObject.SetActive(true);
        }
        
        public void HideIpadViewPanel()
        {
            ipadViewPanel.gameObject.SetActive(false);
        }

        public void DisableIpadButton(bool isDisable)
        {
            gamePlayPanel.IpadBtnSwitch(isDisable);
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