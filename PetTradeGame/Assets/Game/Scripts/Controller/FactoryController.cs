using System.Collections.Generic;
using Game.Scripts.Factory;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class FactoryController : MonoBehaviour
    {
        private readonly List<GameObject> _instances = new();

        public void ProduceDocument(List<string> list)
        {
            foreach (string document in list)
            {
                GameObject obj = DocumentFactory.CreateDocument(document);
                float x = Random.Range(-3, 3);
                float y = Random.Range(-3, 3);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.SetActive(true);
                _instances.Add(obj);
            }
        }

        public void ReleaseInstances()
        {
            for (int i = _instances.Count - 1; i >= 0; --i)
                Destroy(_instances[i]);
            _instances.Clear();
        }
    }
}
