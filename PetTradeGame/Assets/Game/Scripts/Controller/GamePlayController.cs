using System;
using Game.Scripts.Objects;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GamePlayController : MonoBehaviour
    {
        private const float WorldTimeThreshold = 1.3f;

        public static bool IsDebugMode;
        
        [SerializeField] private Clock clock;
        [SerializeField] private float targetTime = 360f;
        [SerializeField] private float debugTargetTime = 20f;
        private bool _isTimerStop, _isReadyFinish, _isAlreadyCompliant;

        private int _score, _correctDocuments, _wrongDocuments;
        private float _time, _timeThreshold, _npcTime;
        
        public static event EventHandler GameFinishEvent, StopProduceDocument, ClearConversationEvent, StartCompliantEvent;

        private void Awake()
        {
            enabled = false;
        }

        public void Reset()
        {
            _timeThreshold = 0f;
            _time = 0f;
            _score = 0;
            _correctDocuments = 0;
            _wrongDocuments = 0;
            clock.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isTimerStop)
            {
                UpdateTime();
                RecordNpcTime();
            }

            if (!_isReadyFinish)
                return;
            
            
            StopProduceDocument?.Invoke(this, EventArgs.Empty);
            if (!(_timeThreshold >= WorldTimeThreshold))
            {
                _timeThreshold += Time.deltaTime;
                return;
            }

            _isReadyFinish = false;
            GameFinishEvent?.Invoke(this, EventArgs.Empty);
            ClearConversationEvent?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTime()
        {
            switch (IsDebugMode)
            {
                case true when _time >= debugTargetTime:
                    _time = debugTargetTime;
                    FinishedCall();
                    return;
                case false when _time >= targetTime:
                    _time = targetTime;
                    FinishedCall();
                    return;
            }
            
            _time += Time.deltaTime;
            clock.UpdateTime(_time);
        }

        private void FinishedCall()
        {
            _isReadyFinish = true;
            _isTimerStop = true;
        }

        private void RecordNpcTime()
        {
            if (!(_npcTime >= 10f))
            {
                if(_isAlreadyCompliant)
                    return;

                _npcTime += Time.deltaTime;
                return;
            }

            _npcTime = 0;
            _isTimerStop = true;
            _isAlreadyCompliant = true;
            StartCompliantEvent?.Invoke(this, EventArgs.Empty);
        }

        public void CalculateScore(int index, int score, bool isWrongDoc)
        {
            switch (index)
            {
                //Approved Correct Document
                case 0 when !isWrongDoc:
                    _score += score;
                    _correctDocuments++;
                    Debug.Log($"Approved Correct Document, score : {_score}, Correct Count : {_correctDocuments}");
                    break;
                //Approved Wrong Document
                case 0 when true:
                    _score -= score;
                    _wrongDocuments++;
                    Debug.Log($"Approved Wrong Document, score : {_score}, Fail Count : {_wrongDocuments}");
                    break;
                //Rejected Correct Document
                case 1 when !isWrongDoc:
                    _score -= score;
                    _wrongDocuments++;
                    Debug.Log($"Rejected Wrong Document, score : {_score}, Fail Count : {_wrongDocuments} ");
                    break;
                //Rejected Wrong Document
                case 1 when true:
                    _score += score;
                    _correctDocuments++;
                    Debug.Log($"Rejected Correct Document, score : {_score}, Correct Count : {_correctDocuments}");
                    break;
            }
        }

        public void SetTimer(bool isStop)
        {
            if (!isStop)
                clock.gameObject.SetActive(true);

            _isTimerStop = isStop;
        }

        public void HasCompliant(bool hasComplaint)
        {
            _isAlreadyCompliant = hasComplaint;
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

        public void Init()
        {
            enabled = true;
            clock.gameObject.SetActive(true);
        }
    }
}