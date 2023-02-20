using System;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        public static bool IsDebugMode = false;
        public static event EventHandler StartGameEvent, StartTutorialEvent, SelectLevelEvent, NextDayEvent;

        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private GamePlayPanel gamePlayPanel;
        [SerializeField] private IpadViewPanel ipadViewPanel;
        [SerializeField] private SettingPanel settingPanel;
        [SerializeField] private EndGamePanel endGamePanel;
        
        private Canvas _canvas;
        
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
        
        public void HideGameplayPanel()
        {
            gamePlayPanel.gameObject.SetActive(false);
        }

        public void ShowEndGamePanel()
        {
            gamePlayPanel.gameObject.SetActive(false);
            endGamePanel.gameObject.SetActive(true);
        }
        
        public void OnBtnStartClicked()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnBtnTutorialClicked()
        {
            StartTutorialEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnBtnSelectLvClicked()
        {
            mainMenuPanel.menu.SetActive(false);
            mainMenuPanel.selectLv.SetActive(true);
            SelectLevelEvent?.Invoke(this, EventArgs.Empty);
        }

        /*public void OnBtnLvBackClicked()
        {
            
        }*/

        public void OnBtnLvClicked()
        {
            mainMenuPanel.gameObject.SetActive(false);
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

        public void SetAnimalGuide(Sprite sprite)
        {
            ipadViewPanel.animalGuide.sprite = sprite;
            ipadViewPanel.animalGuideView.SetActive(true);
        }
        
        public void OnBtnSettingClicked()
        {
            settingPanel.gameObject.SetActive(true);
            if(settingPanel.confirmView.activeSelf)
                settingPanel.confirmView.SetActive(false);
            
            settingPanel.settingView.SetActive(true);
        }

        public void OnBtnResumeClicked()
        {
            settingPanel.confirmView.SetActive(false);
            settingPanel.gameObject.SetActive(false);
        }
        
        //TODO: Frame rate setting

        #endregion
    }
}