using System;
using UnityEngine;

namespace Game.Scripts.TempCode
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private Transform hourPivot, minutePivot;

        private const float HoursToDegree = -0.66f, MinuteToDegree = -8f;
        
        public void UpdateTime(float time)
        {
            hourPivot.localRotation = Quaternion.Euler(0,0,HoursToDegree * time);
            minutePivot.localRotation = Quaternion.Euler(0, 0, MinuteToDegree * time);
        }
    }
}
