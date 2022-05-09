using System.Collections;
using Game.Scripts.Common.Animation;
using Game.Scripts.Common.UI;
using Game.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class ConversationPanel : MonoBehaviour
    {
        public TextMeshProUGUI message;
        public Image speaker;
        public GameObject arrow;
        public Panel panel;

        private void Start()
        {
            Vector3 pos = arrow.transform.localPosition;
            arrow.transform.localPosition = new Vector3(pos.x, pos.y + 5, pos.z);
            Tweener tweener = arrow.transform.MoveToLocal(new Vector3(pos.x, pos.y - 5, pos.z),
                0.5f, EasingEquations.EaseInOutQuad);
            tweener.easingControl.loopType = EasingControl.LoopType.PingPong;
            tweener.easingControl.loopCount = -1;
        }

        public IEnumerator Display(SpeakerData data)
        {
            speaker.sprite = data.speaker;
            //speaker.SetNativeSize();

            for (int i = 0; i < data.messages.Count; ++i)
            {
                message.text = data.messages[i];
                arrow.SetActive(i + 1 < data.messages.Count);
                yield return null;
            }
        }
    }
}
