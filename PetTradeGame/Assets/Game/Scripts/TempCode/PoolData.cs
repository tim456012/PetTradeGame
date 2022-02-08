using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.TempCode
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
