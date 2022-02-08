using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Game.Scripts.TempCode
{
    public class ObjectPoolDemo : MonoBehaviour
    {
        private const string PoolKey = "Demo.Prefab";
        [SerializeField] private GameObject prefab;
        private List<Poolable> instances = new();

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(GameObjectPoolController.AddEntry(PoolKey, prefab, 10, 15)
                ? "Pre-populating pool" : "Pool already configured");
        }

        void ReleaseInstances()
        {
            for (int i = instances.Count - 1; i >= 0; --i)
                GameObjectPoolController.Enqueue(instances[i]);
            instances.Clear();
        }
    }
}
