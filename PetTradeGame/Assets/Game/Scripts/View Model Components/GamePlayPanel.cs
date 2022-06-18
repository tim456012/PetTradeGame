using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class GamePlayPanel : MonoBehaviour
    {
        public TextMeshProUGUI hourTimeText;
        public Button btnSetting, btnZoomIn, btnZoomOut, btnIpadOn, btnIpadOff;
        
        private void Start()
        {

        }
        
        public void SetTime(string text)
        {
            hourTimeText.text = text;
        }
    }
}
