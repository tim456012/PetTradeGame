using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.View_Model_Components
{
    public class GamePlayPanel : MonoBehaviour
    {
        public TextMeshProUGUI score, hourTimeText;
        
        private void Start()
        {

        }

        public void SetScore(int s)
        {
            score.text = $"Score : {s}";
        }

        public void SetTime(string text)
        {
            hourTimeText.text = text;
        }
    }
}
