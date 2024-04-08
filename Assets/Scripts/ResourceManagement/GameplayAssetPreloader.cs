using System;
using System.Collections.Generic;
using System.Linq;
using ResourceManagement.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace ResourceManagement
{
    public class GameplayAssetPreloader
    {
        private Dictionary<string, AsyncOperationHandle<ScriptableObject>> _loadingPool = new();


        public void StartPreloadingAsset(string addressableName, Action<ScriptableObject> callbackAction)
        {
            if (_loadingPool.ContainsKey(addressableName) && _loadingPool[addressableName].IsValid())
            {
                if (_loadingPool[addressableName].IsDone)
                    callbackAction?.Invoke(_loadingPool[addressableName].Result);
                else
                    _loadingPool[addressableName].Completed += handle => callbackAction?.Invoke(handle.Result);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<ScriptableObject>(addressableName);
                
                _loadingPool.Add(addressableName, handle);
                
                handle.Completed += h => { callbackAction?.Invoke(h.Result); };
            }
        }
    }
}