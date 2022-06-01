using UnityEngine;
using TMPro;

namespace Game.Scripts.View_Model_Components
{
    //TODO: UI Animation
    public class EndGamePanel : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;

        public void SetScore(int s)
        {
            scoreText.text = s.ToString();
        }
    }
}
