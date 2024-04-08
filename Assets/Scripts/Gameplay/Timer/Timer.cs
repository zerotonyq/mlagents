using System;
using UnityEngine;
using Zenject;

namespace Timer
{
    public class Timer : ITickable
    {
        private float _currentTime = 0f;
        
        private float _time;
        
        private bool _paused = true;

        public Action EndTimer;
        public Action<float> TimeUpdated;
        
        public void StartTimer() => _paused = false;
        
        public void StopTimer() => _paused = true;

        public void ResetCurrentTime() => _currentTime = 0f;

        public void SetTime(float time)
        {
            _time = time >= 0f ? time : 0f;
            _currentTime = 0f;
        } 

        public float CurrentTime => _currentTime;

        public bool Paused => _paused;
        
        public void Tick()
        {
            if (_paused)
                return;
            
            _currentTime += Time.deltaTime;
            
            TimeUpdated?.Invoke(_time - _currentTime);
            
            if (_currentTime >= _time)
            {
                TimeUpdated?.Invoke(0f);
                StopTimer();
                ResetCurrentTime();
                EndTimer?.Invoke();
            }
        }
    }
}