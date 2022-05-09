using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts
{
    public interface IHasChanged : IEventSystemHandler
    {
        public void HasChanged();
    }

    public class Inventory : MonoBehaviour, IHasChanged
    {
        [SerializeField] private Transform slots;
        [SerializeField] private TextMeshProUGUI itemText;

        // Start is called before the first frame update
        void Start()
        {
            HasChanged();
        }

        public void HasChanged()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" - ");
            foreach (Transform slot in slots)
            {
                GameObject item = slot.GetComponent<Slot>().item;
                if (!item)
                    continue;
                sb.Append(item.name);
                sb.Append(" - ");
            }
            itemText.text = sb.ToString();
        }
    }
}
