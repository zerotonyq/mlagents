﻿using System;
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
        private Dictionary<string, AsyncOperationHandle<ScriptableObject>> _scriptableObjectLoadingPool = new();
        private Dictionary<string, AsyncOperationHandle<GameObject>> _gameObjectLoadingPool = new();

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
    }
}