using Game.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.Scripts.TempCode
{
    public class ScaleTest : MonoBehaviour
    {
        public DragAndDropController dragAndDropController;
        public Button button;
        private bool _isScaled;

        private GameObject _lastObj;

        // Start is called before the first frame update
        private void Start()
        {
        }

        public void ScaleDocument()
        {
            GameObject obj = dragAndDropController.LastObj;
            if (obj == null)
                return;
            if (_lastObj == null)
                _lastObj = obj;

            _isScaled = !_isScaled;
            if (_lastObj != obj)
            {
                _lastObj.transform.localScale = new Vector3(0.3f, 0.3f, 0);
                _lastObj.GetComponent<SortingGroup>().sortingLayerName = "Default";
                _lastObj = obj;
            }

            var sg = obj.GetComponent<SortingGroup>();
            if (_isScaled)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = "Put down";
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
                sg.sortingLayerName = "SelectedObjects";
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = "Pick up";
                obj.transform.localScale = new Vector3(0.3f, 0.3f, 0);
                sg.sortingLayerName = "Default";
            }
        }
    }
}