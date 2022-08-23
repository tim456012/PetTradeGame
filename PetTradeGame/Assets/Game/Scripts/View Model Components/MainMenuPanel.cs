using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class MainMenuPanel : MonoBehaviour
    {
        public Button btnStart, btnTutorial;

        private void Start()
        {
            btnStart.onClick.AddListener(OnBtnStartClicked);
            btnTutorial.onClick.AddListener(OnBtnTutorialClicked);
        }

        private void OnBtnStartClicked()
        {
            Debug.Log("Start Game");
            gameObject.SetActive(false);
        }

        private void OnBtnTutorialClicked()
        {
            Debug.Log("Start Tutorial");
            gameObject.SetActive(false);
        }
    }
}