using System;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class GamePlayController : MonoBehaviour
    {
        private int _score;

        private void Awake()
        {
            enabled = false;
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
    }
}
