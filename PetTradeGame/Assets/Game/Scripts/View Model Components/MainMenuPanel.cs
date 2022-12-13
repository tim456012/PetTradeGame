using Game.Scripts.Controller;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class MainMenuPanel : MonoBehaviour
    {
        public GameObject menu, selectLv;
        public Button btnStart, btnTutorial, btnSelectLv, btnSetting;
        public Button btnLvBack;

        private UIController _controller;

        private void Awake()
        {
            _controller = UIFinder.GetObjComponent<UIController>(transform.parent.parent.gameObject, "UI Controller");
        }

        private void Start()
        {
            btnStart.onClick.AddListener(OnBtnStartClicked);
            btnTutorial.onClick.AddListener(OnBtnTutorialClicked);
            btnSelectLv.onClick.AddListener(OnBtnSelectLvClicked);
            btnSetting.onClick.AddListener(OnBtnSettingClicked);
            btnLvBack.onClick.AddListener(OnBtnLvBackClicked);
        }

        private void OnBtnStartClicked()
        {
            Debug.Log("Start Game");
            gameObject.SetActive(false);
            _controller.OnBtnStartClicked();
        }

        private void OnBtnTutorialClicked()
        {
            Debug.Log("Start Tutorial");
            gameObject.SetActive(false);
            _controller.OnBtnTutorialClicked();
        }

        private void OnBtnSelectLvClicked()
        {
            Debug.Log("Start Tutorial");
            _controller.OnBtnSelectLvClicked();
        }

        private void OnBtnSettingClicked()
        {
            Debug.Log("Open Setting");
            _controller.OnBtnSettingClicked();
        }

        private void OnBtnLvBackClicked()
        {
            selectLv.SetActive(false);
            menu.SetActive(true);
        }
    }
}