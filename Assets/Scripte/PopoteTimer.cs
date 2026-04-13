using System;
using UnityEngine;

public class PopoteTimer
{
    public EventHandler OnTimerEnd;
    public EventHandler OnTimerReset;
    
    private float _timer;
    private float _waitTime;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;
    public float T => _timer / _waitTime;

    public PopoteTimer(float timer) {
        _waitTime = timer;
    }

    public void Pause() {
        _isPlaying = false;
    }

    public void Play(float _newWaitTime = -1) {
        if (_newWaitTime != -1) {
            ChangeWaitTimer(_newWaitTime); 
        }
        _timer = 0;
        _isPlaying = true;
    }

    public void ChangeWaitTimer(float time) {
        if (IsPlaying) {
            if (time <= _timer) {
                _isPlaying = false;
                OnTimerEnd?.Invoke(this , EventArgs.Empty);  
            }
        }
        _waitTime = time;
    }
    
    public void UpdateTimer() {
        if( !IsPlaying)return;
        _timer += Time.deltaTime;
        if (_timer >= _waitTime) {
            _isPlaying = false;
            OnTimerEnd?.Invoke(this , EventArgs.Empty);
        }
    }

    public void ResetTimer() {
        _timer = 0;
        OnTimerReset?.Invoke(this, EventArgs.Empty);
    }
    
}