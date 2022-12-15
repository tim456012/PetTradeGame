using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Objects
{
    public class Clock : MonoBehaviour
    {
        private TextMeshProUGUI timeText;
        
        //private const float HoursToDegree = -0.66f, MinuteToDegree = -8f;
        //[SerializeField] private Transform hourPivot, minutePivot;

        private void Start()
        {
            timeText = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateTime(float time)
        {
            timeText.text = $"Time: {(int)time / 60:00}:{(int)time % 60:00}";
            
            //hourPivot.localRotation = Quaternion.Euler(0, 0, HoursToDegree * time);
            //minutePivot.localRotation = Quaternion.Euler(0, 0, MinuteToDegree * time);
        }
    }
}