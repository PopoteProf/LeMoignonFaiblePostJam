using System;
using UnityEngine;

public class UIMeatPile : MonoBehaviour {
    
    [SerializeField] private UIMemberSelection[] _uiMemberSelections;
    [SerializeField] private Transform _mainPanel;
    [SerializeField] private AudioElement _aeOpenPanel;
    
    private void Awake() {
        StaticEvents.OnMeatPileEnter+= StaticEventsOnOnMeatPileEnter;
        foreach (var ui in _uiMemberSelections) {
            ui.OnSelection+= UiOnOnSelection;
        }
    }

    private void OnDestroy() {
        StaticEvents.OnMeatPileEnter-= StaticEventsOnOnMeatPileEnter;
    }

    private void UiOnOnSelection(object sender, EventArgs e)
    {
        _mainPanel.gameObject.SetActive(false);
    }

    private void StaticEventsOnOnMeatPileEnter(object sender, SoMemberList e) {
        foreach (var ui in _uiMemberSelections) {
            ui.SetUpSOMember(e.GetReward());
        }
        _mainPanel.gameObject.SetActive(true);
        AudioBus.OnPlayAudioElementSFX(_aeOpenPanel);
    }
}