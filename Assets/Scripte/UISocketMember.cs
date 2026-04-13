using System;
using UnityEngine;
using UnityEngine.UI;

public class UISocketMember : MonoBehaviour
{
    public event EventHandler<float> OnMemberRotChange; 
    [SerializeField] private Slider _slider;

    private void Awake() {
        _slider.onValueChanged.AddListener(UIOnValueChanged);
    }
    private void UIOnValueChanged(float value) {
        OnMemberRotChange?.Invoke(this, value);
    }
}