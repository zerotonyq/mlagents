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

        private bool _looped = false;

        public Action EndTimer;
        public Action<float> TimeUpdated;
        
        public bool Looped => _looped;
        
        public void StartTimer() => _paused = false;
        
        public void StopTimer() => _paused = true;

        public void SetLooped(bool i) => _looped = i;

        public void ResetCurrentTime() => _currentTime = 0f;

        public void SetTime(float time)
        {
            _time = time >= 0f ? time : 0f;
            _currentTime = 0f;
        }

        public void AddTime(float time)
        {
            if (time < 0)
                throw new ArgumentException("time must not be less than zero");
            _time += time;
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
                
                if(!_looped)
                    StopTimer();
     
                ResetCurrentTime();
                
                EndTimer?.Invoke();
            }
        }
    }
}