/*
using System.Collections.Generic;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GameObject = Game.Scripts.View_Model_Components.GameObject;

namespace Game.Scripts.Controller
{
    /// <summary>
    /// This controller will hold a PoolData dictionary as a object pool to process the instantiation.
    /// </summary>
    public class GameObjectPoolController : MonoBehaviour
    {
        #region Fields / Properties
        private static GameObjectPoolController Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateSharedInstance();
                    Debug.Log("Instance created.");
                }

                return _instance;
            }
        }
        private static GameObjectPoolController _instance;
        private readonly static Dictionary<string, PoolData> Pools = new Dictionary<string, PoolData>();
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Modify the max amount of specific objects in the dictionary. 
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary.</param>
        /// <param name="maxCount">The maximum number of GameObjects that need to keep in the pool.</param>
        public static void SetMaxCount(string key, int maxCount)
        {
            if (!Pools.ContainsKey(key))
                return;
            PoolData data = Pools[key];
            data.MaxCount = maxCount;
        }

        /// <summary>
        /// Specify what name (key) will map to the specific GameObjects and add it to the dictionary.
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary that you want.</param>
        /// <param name="prefab">The GameObjects that you want to place on the object pool.</param>
        /// <param name="prePopulate">The specific number of prefab that you want to pre-create them in the object pool.</param>
        /// <param name="maxCount">The maximum number of prefab that can keep in the object pool.</param>
        /// <returns>bool</returns>
        public static bool AddEntry(string key, UnityEngine.GameObject prefab, int prePopulate, int maxCount)
        {
            if (Pools.ContainsKey(key))
                return false;

            PoolData data = new PoolData
            {
                Prefab = prefab,
                MaxCount = maxCount,
                Pool = new Queue<GameObject>(prePopulate)
            };
            Pools.Add(key, data);

            for (int i = 0; i < prePopulate; ++i)
            {
                Enqueue(CreateInstance(key, prefab));
            }

            return true;
        }

        /// <summary>
        /// Clear and remove the specific GameObjects in the object pools.
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary.</param>
        public static void ClearEntry(string key)
        {
            if (!Pools.ContainsKey(key))
                return;

            PoolData data = Pools[key];
            while (data.Pool.Count > 0)
            {
                GameObject obj = data.Pool.Dequeue();
                UnityEngine.GameObject.Destroy(obj.gameObject);
            }
            Pools.Remove(key);
        }

        /// <summary>
        /// Place (Enqueue) the GameObjects back from the game world to the object pools.
        /// </summary>
        /// <param name="sender">The GameObject that you want to send to the object pool.</param>
        public static void Enqueue(GameObject sender)
        {
            if (sender == null || sender.isPooled || !Pools.ContainsKey(sender.key))
                return;

            PoolData data = Pools[sender.key];
            if (data.Pool.Count >= data.MaxCount)
            {
                UnityEngine.GameObject.Destroy(sender.gameObject);
                return;
            }

            data.Pool.Enqueue(sender);
            sender.isPooled = true;
            sender.transform.SetParent(Instance.transform);
            sender.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get (Dequeue) the GameObjects from the object pools to the game world.
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary.</param>
        /// <returns>GameObject</returns>
        public static GameObject Dequeue(string key)
        {
            if (!Pools.ContainsKey(key))
                return null;

            PoolData data = Pools[key];
            if (data.Pool.Count == 0)
                return CreateInstance(key, data.Prefab);

            GameObject obj = data.Pool.Dequeue();
            obj.isPooled = false;
            return obj;
        }
        #endregion

        #region Private
        private static void CreateSharedInstance()
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject("GameObject Pool Controller");
            DontDestroyOnLoad(obj);
            _instance = obj.AddComponent<GameObjectPoolController>();
        }

        private static GameObject CreateInstance(string key, UnityEngine.GameObject prefab)
        {
            UnityEngine.GameObject gameObject = Instantiate(prefab);
            GameObject p = gameObject.AddComponent<GameObject>();
            p.key = key;
            return p;
        }
        #endregion
    }
}
*/
