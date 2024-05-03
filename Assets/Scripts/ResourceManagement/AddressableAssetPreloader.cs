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
    public class AddressableAssetPreloader
    {
        private Dictionary<string, AsyncOperationHandle<ScriptableObject>> _scriptableObjectLoadingPool = new();
        private Dictionary<string, AsyncOperationHandle<GameObject>> _gameObjectLoadingPool = new();
        private Dictionary<string, AsyncOperationHandle<Material>> _materialLoadingPool = new();

        public void StartPreloadingAsset(string addressableName, Action<ScriptableObject> callbackAction)
        {
            if (_scriptableObjectLoadingPool.ContainsKey(addressableName) && _scriptableObjectLoadingPool[addressableName].IsValid())
            {
                if (_scriptableObjectLoadingPool[addressableName].IsDone)
                    callbackAction?.Invoke(_scriptableObjectLoadingPool[addressableName].Result);
                else
                    _scriptableObjectLoadingPool[addressableName].Completed += handle => callbackAction?.Invoke(handle.Result);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<ScriptableObject>(addressableName);
                
                _scriptableObjectLoadingPool.Add(addressableName, handle);
                
                handle.Completed += h => { callbackAction?.Invoke(h.Result); };
            }
        }

        public void StartPreloadingAsset(string addressableName, Action<GameObject> callbackAction)
        {
            if (_gameObjectLoadingPool.ContainsKey(addressableName) && _gameObjectLoadingPool[addressableName].IsValid())
            {
                if (_gameObjectLoadingPool[addressableName].IsDone)
                    callbackAction?.Invoke(_gameObjectLoadingPool[addressableName].Result);
                else
                    _gameObjectLoadingPool[addressableName].Completed +=
                        handle => callbackAction?.Invoke(handle.Result);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(addressableName);
                
                _gameObjectLoadingPool.Add(addressableName, handle);

                handle.Completed += h => callbackAction?.Invoke(h.Result);
            }
        }
        
        public void StartPreloadingAsset(string addressableName, Action<Material> callbackAction)
        {
            if (_materialLoadingPool.ContainsKey(addressableName) && _materialLoadingPool[addressableName].IsValid())
            {
                if (_materialLoadingPool[addressableName].IsDone)
                    callbackAction?.Invoke(_materialLoadingPool[addressableName].Result);
                else
                    _materialLoadingPool[addressableName].Completed +=
                        handle => callbackAction?.Invoke(handle.Result);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<Material>(addressableName);
                
                _materialLoadingPool.Add(addressableName, handle);

                handle.Completed += h => callbackAction?.Invoke(h.Result);
            }
        }
    }
}