using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Game.Scripts.Tools
{ /*  This script is a helper tool for the Addressable system. It is use to achieve multiple load / unload assets with a single call.
    Topic related and source from: https://forum.unity.com/threads/1-15-1-assetreference-not-allow-loadassetasync-twice.959910/page-2#post-7306750
    There have 3 way to load asset:
    A. 1 to 1 (Load once and Release once)
    B. N to 1 (Load N time and Release once)
    C. N to N (Load N time and Release N time)
    
    However, B and C method may cause error if you load asset more than twice without safety check and management.
    Thus this script is use to handle this problem in safety way.
*/

    /// <summary>
    /// This class is use to achieve B and C method: "N to 1" & "N to N". Need to define reference and use with <see cref="AssetReference"/>.
    /// </summary>
    [Serializable]
    public class AutoCountedAssetRef
    {
        [SerializeField] private AssetReference assetReference;
        [SerializeField] private string assetAddressKey;
 
        private readonly Stack<AsyncOperationHandle> _handles = new Stack<AsyncOperationHandle>();
 
        public int RefCount => _handles.Count;
    
        public Task<T> LoadAsync<T>(AssetReference assetRef) where T : Object
        {
            assetReference = assetRef;
            return LoadAsyncInternal<T>().Task.ContinueWith(obj => (T)obj.Result);
        }
        
        public Task<T> LoadAsync<T>(string assetRefKey) where T : Object
        {
            assetAddressKey = assetRefKey;
            return LoadKeyAsyncInternal<T>().Task.ContinueWith(obj => (T)obj.Result);
        }
 
        public IEnumerator LoadAsyncCoroutine<T>() where T : Object
        {
            return LoadAsyncInternal<T>();
        }
 
        public void Release()
        {
            if (_handles.Count > 0)
            {
                var handle = _handles.Pop();
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogWarning("Attempting to Release AutoCountedAssetRef, but the count is already at 0!");
            }
        }
 
        private AsyncOperationHandle LoadAsyncInternal<T>() where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetReference.RuntimeKey);
            _handles.Push(handle);
            return handle;
        }
        
        private AsyncOperationHandle LoadKeyAsyncInternal<T>() where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetAddressKey);
            _handles.Push(handle);
            return handle;
        }
    }
}