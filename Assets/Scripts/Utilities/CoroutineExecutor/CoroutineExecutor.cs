using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.CoroutineExecutor
{
    public class CoroutineExecutor : MonoBehaviour
    {
        public UnityEvent Executed = new();
        public void Execute(Action<int> action, int count, float delayTime)
        {
            Executed.RemoveAllListeners();
            StartCoroutine(ExecuteCoroutine(action, count, delayTime));
        }

        public IEnumerator ExecuteCoroutine(Action<int> action, int count, float delayTime)
        {
            for (int i = 0; i < count; i++)
            {
                action(i);
                yield return new WaitForSeconds(delayTime);
            }           
            Executed?.Invoke();
            
            yield break;
        }
        
    
    }
}