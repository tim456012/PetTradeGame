using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GamePlayController : MonoBehaviour
    {
        private const float TargetTime = 360f;
        private const float UpdateTimeThreshold = 11.25f; //360 / (8 * 4)

        private int _score, _h = 9, _m;
        private float _time, _updateTime;

        public static event EventHandler<InfoEventArgs<string>> TimerEvent;
        
        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if(_time >= TargetTime)
                _time = TargetTime;
            else
            {
                _time += Time.deltaTime;
                _updateTime += Time.deltaTime;
            }
            
            DisplayTime(_time);
        }

        public int CalculateScore(sbyte index, int score)
        {
            switch (index)
            {
                case 0:
                    _score += score;
                    break;
                case 1:
                    _score -= score;
                    break;
            }
            
            return _score;
        }
        
        private void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay >= TargetTime)
                return;

            //float minutes = Mathf.FloorToInt(timeToDisplay / 60f);
            //float seconds = Mathf.FloorToInt(timeToDisplay % 60f);
            
            //timeText.text = $"{minutes:00}:{seconds:00}";

            if (!(_updateTime >= UpdateTimeThreshold))
                return;
            
            _m += 15;
            if (_m == 60)
            {
                _h++;
                _m = 0;
            }

            string text = $"{_h:00}:{_m:00}";
            _updateTime = 0;
            
            TimerEvent?.Invoke(this, new InfoEventArgs<string>(text));
        }

    }
}
