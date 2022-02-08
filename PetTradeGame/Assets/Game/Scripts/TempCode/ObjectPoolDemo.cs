using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.TempCode
{
    public class ObjectPoolDemo : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        private const string PoolKey = "Document.Prefab";
        private const int ObjCount = 5;
        
        private List<Poolable> instances = new();

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(GameObjectPoolController.AddEntry(PoolKey, prefab, 10, 15)
                ? "Pre-populating pool" : "Pool already configured");
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Dequeue"))
            {
                Poolable obj = GameObjectPoolController.Dequeue(PoolKey);
                float x = UnityEngine.Random.Range(-10, 10);
                float y = UnityEngine.Random.Range(0, 5);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.gameObject.SetActive(true);
                instances.Add(obj);
            }

            if (GUI.Button(new Rect(10, 50, 100, 30), "Enqueue"))
            {
                if (instances.Count <= 0)
                    return;
                Poolable obj = instances[0];
                instances.RemoveAt(0);
                GameObjectPoolController.Enqueue(obj);
            }
        }

        void ReleaseInstances()
        {
            for (int i = instances.Count - 1; i >= 0; --i)
                GameObjectPoolController.Enqueue(instances[i]);
            instances.Clear();
        }
    }
}
