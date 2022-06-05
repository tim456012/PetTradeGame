using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class MainMenuPanel : MonoBehaviour
    {
        public Button btnStart;
        
        private void Start()
        {
            btnStart.onClick.AddListener(OnBtnStartClicked);
        }

        private void OnBtnStartClicked()
        {
            Debug.Log("Start Game");
            gameObject.SetActive(false);
        }
    }
}
