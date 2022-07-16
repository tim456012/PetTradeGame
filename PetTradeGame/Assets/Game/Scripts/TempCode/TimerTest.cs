using TMPro;
using UnityEngine;

namespace Game.Scripts.TempCode
{
    public class TimerTest : MonoBehaviour
    {
        //Working Hours: for UI only
        //Time: 6 minutes
        //working hours second / time second
        //time second / 3 answer

        private const float TargetTime = 360f;

        //360 / (8 * 4)
        private const float UpdateTimeThreshold = 11.25f;

        public TextMeshProUGUI timeText, hourTimeText;
        private int _h = 9, _m;

        private float _time, _updateTime;

        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (_time >= TargetTime)
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
            if (timeToDisplay >= TargetTime)
                timeToDisplay = TargetTime;

            float minutes = Mathf.FloorToInt(timeToDisplay / 60f);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60f);

            timeText.text = $"{minutes:00}:{seconds:00}";

            if (!(_updateTime >= UpdateTimeThreshold))
                return;

            _m += 15;
            if (_m == 60)
            {
                _h++;
                _m = 0;
            }

            hourTimeText.text = $"{_h:00}:{_m:00}";
            _updateTime = 0;
        }
    }
}