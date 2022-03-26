using System.Collections.Generic;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Model
{
    /// <summary>
    /// The information of objects in the object pool.
    /// </summary>
    public class PoolData
    {
        public GameObject prefab;
        public int maxCount;
        public Queue<Poolable> pool;
    }
}
