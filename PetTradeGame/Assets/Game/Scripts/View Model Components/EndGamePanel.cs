using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    //TODO: UI Animation
    public class EndGamePanel : MonoBehaviour
    {
        public TextMeshProUGUI correctDocText, wrongDocText, scoreText;
        public Button btnNextDay;

        private void Start()
        {
            btnNextDay.onClick.AddListener(OnBtnNextDayClicked);
        }

        private void OnBtnNextDayClicked()
        {
            Debug.Log("Go to next day");
            gameObject.SetActive(false);
        }
    }
}