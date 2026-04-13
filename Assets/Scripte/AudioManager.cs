using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance;

    [SerializeField] private float _defaultMusicFade = 1;
    [SerializeField] private float _DefaultAmbianceFade = 1;
    [SerializeField] private AudioMixerGroup _sfxAudioMixer;
    [SerializeField] private AudioMixerGroup _MusicAudioMixer;
    [SerializeField] private AudioMixerGroup _AmbianceAudioMixer;
    [SerializeField] private AudioClip _testAudioClip;
    
    private List<FadeInAndOutAudiosource> _fadeOutAudiosources = new List<FadeInAndOutAudiosource>();
    private AudioSource _musicAudioSource;
    private AudioSource _ambianceAudioSource;
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        AudioBus.OnPlayAudioElementSFX += OnPlayAudioElementSfx;
        AudioBus.OnPlayAudioElementAmbiance += OnPlayAudioElementAmbiance;
        AudioBus.OnPlayAudioElementMusic += OnPlayAudioElementMusic;
    }

    private void OnDestroy() {
        AudioBus.OnPlayAudioElementSFX -= OnPlayAudioElementSfx;
        AudioBus.OnPlayAudioElementAmbiance -= OnPlayAudioElementAmbiance;
        AudioBus.OnPlayAudioElementMusic -= OnPlayAudioElementMusic;
    }

    private void Update() {
        for (int i = _fadeOutAudiosources.Count - 1; i >= 0; i--) {
            _fadeOutAudiosources[i].Update();
            if (_fadeOutAudiosources[i].IsComplet) {
                Debug.Log("Fade Out element is complete");
                if( _fadeOutAudiosources[i].CanBeDestroy) _fadeOutAudiosources[i].DestroyAudioSource();
                _fadeOutAudiosources.Remove(_fadeOutAudiosources[i]);
            }
        }
    }

    private void OnPlayAudioElementMusic(AudioElement audioElement)
    {
        if (_musicAudioSource != null) {
            if( audioElement.GetAudioClip()==_musicAudioSource.clip)return;
            _fadeOutAudiosources.Add(new FadeInAndOutAudiosource(_musicAudioSource, _defaultMusicFade, 0, false));
        }
        _musicAudioSource= gameObject.AddComponent<AudioSource>();
        _musicAudioSource.volume = 0;
        _fadeOutAudiosources.Add(new FadeInAndOutAudiosource(_musicAudioSource, _defaultMusicFade, audioElement.Volume, true));
        _musicAudioSource.clip = audioElement.GetAudioClip();
        _musicAudioSource.outputAudioMixerGroup = _MusicAudioMixer;
        _musicAudioSource.loop = true;
        _musicAudioSource.Play();
    }

    private void OnPlayAudioElementAmbiance(AudioElement audioElement)
    {
        if( audioElement.AudioClips.Length==0 &&_ambianceAudioSource==null) return;
        if (_ambianceAudioSource != null) {
            if (audioElement.AudioClips.Length!=0&&audioElement.GetAudioClip()== _ambianceAudioSource.clip)return;
            _fadeOutAudiosources.Add(new FadeInAndOutAudiosource(_ambianceAudioSource, _DefaultAmbianceFade, 0, false));
        }
        
        _ambianceAudioSource= gameObject.AddComponent<AudioSource>();
        _ambianceAudioSource.volume = 0;
        _fadeOutAudiosources.Add(new FadeInAndOutAudiosource(_ambianceAudioSource, _DefaultAmbianceFade, audioElement.Volume, true));
        _ambianceAudioSource.clip = audioElement.GetAudioClip();
        _ambianceAudioSource.outputAudioMixerGroup = _AmbianceAudioMixer;
        _ambianceAudioSource.loop = true;
        _ambianceAudioSource.Play();
    }

    private void OnPlayAudioElementSfx(AudioElement audioElement) {
        audioElement.PlaySFXOnGameObject(gameObject, _sfxAudioMixer);
    }

    private class FadeInAndOutAudiosource {
        AudioSource _audioSource;
        PopoteTimer _fadeOutTimer;
        private float _startVomule;
        private bool _isFadeIn;
        private float _targetVomule;
        public bool IsComplet;
        public bool CanBeDestroy;

        public FadeInAndOutAudiosource(AudioSource audioSource, float fadeTime, float targetVolume, bool isFadeIn) {
            _audioSource = audioSource;
            _startVomule = _audioSource.volume;
            CanBeDestroy = false;
            IsComplet = false;
            _targetVomule = targetVolume;
            _isFadeIn = isFadeIn;
            _fadeOutTimer = new PopoteTimer(fadeTime);
            _fadeOutTimer.Play();
        }

        public void Update() {
            _fadeOutTimer.UpdateTimer();
            _audioSource.volume = Mathf.Lerp(_startVomule, _targetVomule, _fadeOutTimer.T);
            
            if (_fadeOutTimer.T >=1) {
                IsComplet = true;
                if (!_isFadeIn) CanBeDestroy = true;
            }
        }

        public void DestroyAudioSource() {
            Destroy(_audioSource);
        }
    }
}