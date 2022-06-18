using System;
using Game.Scripts.EventArguments;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GamePlayController : MonoBehaviour
    {
        private const float UpdateTimeThreshold = 11.25f; //360 / (8 * 4)
        
        [SerializeField] private float targetTime = 365f;
        [SerializeField] private float debugTargetTime = 20f;

        private int _score, _h, _m, _correctDocuments, _wrongDocuments;
        private float _time, _updateTime;
        private bool _startTimer, _isTimerStop, _isDebugMode;

        public static event EventHandler<InfoEventArgs<string>> TimerEvent;
        public static event EventHandler GameFinishEvent;
        
        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            _h = 9;
            _m = 0;
            string text = $"{_h:00}:{_m:00}";
            TimerEvent?.Invoke(this, new InfoEventArgs<string>(text));
        }

        private void Update()
        {
            if (_startTimer && !_isTimerStop)
                UpdateTime();
        }

        //TODO: Need to change logic
        public void CalculateScore(int index, int score)
        {
            Debug.Log(index);
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

        }

        private void UpdateTime()
        {
            switch (_isDebugMode)
            {
                case true when _time >= debugTargetTime:
                    _time = debugTargetTime;
                    _startTimer = false;
                    GameFinishEvent?.Invoke(this, EventArgs.Empty);
                    return;
                case false when _time >= targetTime:
                    _time = targetTime;
                    _startTimer = false;
                    GameFinishEvent?.Invoke(this, EventArgs.Empty);
                    return;
            }
            
            _time += Time.deltaTime;
            _updateTime += Time.deltaTime;
            DisplayTime();
        }

        private void DisplayTime()
        {
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

        public void SetTimerStop()
        {
            _isTimerStop = true;
        }

        public void SetDebugMode()
        {
            _isDebugMode = true;
        }
        
        public void Reset()
        {
            _h = 9;
            _m = 0;
            _time = 0f;
            _updateTime = 0f;
            _score = 0;
            _correctDocuments = 0;
            _wrongDocuments = 0;
        }
    }
}
