using System.Collections.Generic;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    /// <summary>
    /// This controller will hold a PoolData dictionary as a object pool to process the instantiation.
    /// </summary>
    public class GameObjectPoolSubController : MonoBehaviour
    {
        #region Fields / Properties
        private static GameObjectPoolSubController Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateSharedInstance();
                }

                return instance;
            }
        }
        private static GameObjectPoolSubController instance;
        private static readonly Dictionary<string, PoolData> pools = new();
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
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
            if (!pools.ContainsKey(key))
                return;
            PoolData data = pools[key];
            data.maxCount = maxCount;
        }

        /// <summary>
        /// Specify what name (key) will map to the specific GameObjects and add it to the dictionary.
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary that you want.</param>
        /// <param name="prefab">The GameObjects that you want to place on the object pool.</param>
        /// <param name="prePopulate">The specific number of prefab that you want to pre-create them in the object pool.</param>
        /// <param name="maxCount">The maximum number of prefab that can keep in the object pool.</param>
        /// <returns>bool</returns>
        public static bool AddEntry(string key, GameObject prefab, int prePopulate, int maxCount)
        {
            if (pools.ContainsKey(key))
                return false;

            PoolData data = new PoolData
            {
                prefab = prefab,
                maxCount = maxCount,
                pool = new Queue<Poolable>(prePopulate)
            };
            pools.Add(key, data);

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
            if (!pools.ContainsKey(key))
                return;

            PoolData data = pools[key];
            while (data.pool.Count > 0)
            {
                Poolable obj = data.pool.Dequeue();
                GameObject.Destroy(obj.gameObject);
            }
            pools.Remove(key);
        }

        /// <summary>
        /// Place (Enqueue) the GameObjects back from the game world to the object pools.
        /// </summary>
        /// <param name="sender">The GameObject that you want to send to the object pool.</param>
        public static void Enqueue(Poolable sender)
        {
            if (sender == null || sender.isPooled || !pools.ContainsKey(sender.key))
                return;

            PoolData data = pools[sender.key];
            if (data.pool.Count >= data.maxCount)
            {
                GameObject.Destroy(sender.gameObject);
                return;
            }

            data.pool.Enqueue(sender);
            sender.isPooled = true;
            sender.transform.SetParent(Instance.transform);
            sender.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get (Dequeue) the GameObjects from the object pools to the game world.
        /// </summary>
        /// <param name="key">The name (key) of the objects in the dictionary.</param>
        /// <returns>GameObject</returns>
        public static Poolable Dequeue(string key)
        {
            if (!pools.ContainsKey(key))
                return null;

            PoolData data = pools[key];
            if (data.pool.Count == 0)
                return CreateInstance(key, data.prefab);

            Poolable obj = data.pool.Dequeue();
            obj.isPooled = false;
            return obj;
        }
        #endregion

        #region Private
        private static void CreateSharedInstance()
        {
            GameObject obj = new GameObject("GameObject Pool Controller");
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<GameObjectPoolSubController>();
        }

        private static Poolable CreateInstance(string key, GameObject prefab)
        {
            GameObject gameObject = Instantiate(prefab);
            Poolable p = gameObject.AddComponent<Poolable>();
            p.key = key;
            return p;
        }
        #endregion
    }
}
