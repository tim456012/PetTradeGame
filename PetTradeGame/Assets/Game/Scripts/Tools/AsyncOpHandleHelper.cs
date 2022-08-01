using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.Tools
{
    /// <summary>
    /// This helper class is a extension of LoadAssetAsync method and use to achieve A method: "1 to 1".
    /// </summary>
    public static class AsyncOpHandleHelper
    {
        /// <summary>
        /// Get the AsyncOperationHandle by asset reference.
        /// </summary>
        /// <param name="assetReference"></param>
        /// <typeparam name="T">Object</typeparam>
        /// <returns>AsyncOperationHandle</returns>
        public static AsyncOperationHandle<T> Get<T>(this AssetReference assetReference) where T : Object
        {
            return Addressables.LoadAssetAsync<T>(assetReference.RuntimeKey);
        }
    
        /// <summary>
        /// Get the AsyncOperationHandle by generic asset reference.
        /// </summary>
        /// <param name="assetReference"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AsyncOperationHandle<T> Get<T>(this AssetReferenceT<T> assetReference) where T : Object
        {
            return Addressables.LoadAssetAsync<T>(assetReference.RuntimeKey);
        }
    
        /// <summary>
        /// Get the AsyncOperationHandle by asset's key address.
        /// </summary>
        /// <param name="assetReferenceKey"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AsyncOperationHandle<T> Get<T>(this string assetReferenceKey) where T : Object
        {
            return Addressables.LoadAssetAsync<T>(assetReferenceKey);
        }
    }
}