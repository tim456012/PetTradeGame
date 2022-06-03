using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GamePlayController : MonoBehaviour
    {
        private const float TargetTime = 10f;
        private const float UpdateTimeThreshold = 11.25f; //360 / (8 * 4)

        private int _score, _h = 9, _m, _correctDocuments, _wrongDocuments;
        private float _time, _updateTime;
        private bool _startTimer, _isDebugMode;

        public static event EventHandler<InfoEventArgs<string>> TimerEvent;
        public static event EventHandler GameFinishEvent;
        
        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if(_startTimer && !_isDebugMode)
                UpdateTime();
        }

        public int CalculateScore(sbyte index, int score)
        {
            switch (index)
            {
                case 0:
                    _score += score;
                    _correctDocuments++;
                    break;
                case 1:
                    _score -= score;
                    _wrongDocuments++;
                    break;
            }
            
            return _score;
        }

        private void UpdateTime()
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
        
        private void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay > TargetTime)
            {
                _time = TargetTime;
                _startTimer = false;
                GameFinishEvent?.Invoke(this, EventArgs.Empty);
                return;
            }

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

        public void StartTimer()
        {
            _startTimer = true;
        }

        public int GetScore()
        {
            return _score;
        }

        public int GetCorrectDoc()
        {
            return _correctDocuments;
        }

        public int GetWrongDoc()
        {
            return _wrongDocuments;
        }

        public void SetDebugMode()
        {
            _isDebugMode = true;
        }
    }
}
