using TMPro;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;

        private Canvas canvas;
        //private Tweener transition;

        // Start is called before the first frame update
        private void Start()
        {
            canvas = GetComponentInChildren<Canvas>();

        }

        public void setScore(int s)
        {
            score.text = $"Score : {s}";
        }
    }
}
