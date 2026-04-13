using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _return;
    [SerializeField] private AudioElement _aeOpenPanel;
    
    private void  Start() {
        StaticEvents.OnGameOver+= StaticEventsOnOnGameOver;
        _return.onClick.AddListener(UIButtonReturn);
    }

    private void OnDestroy() {
        StaticEvents.OnGameOver-= StaticEventsOnOnGameOver;
    }

    private void StaticEventsOnOnGameOver(object sender, EventArgs e) {
        _gameOverPanel.SetActive(true);
        AudioBus.OnPlayAudioElementSFX(_aeOpenPanel);
    }

    private void UIButtonReturn() {
        SceneManager.LoadScene(0);
    }
}