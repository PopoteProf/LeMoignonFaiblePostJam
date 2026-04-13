using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMemberSelection : MonoBehaviour {
    
    public event EventHandler OnSelection;
    [SerializeField] private Button _bp;
    [SerializeField] private Image _imgDisplay;
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtType;
    [SerializeField] private TMP_Text _txtDescription;
    [SerializeField] private SoMember _soMember;

    public void SetUpSOMember(SoMember soMember) {
        _soMember = soMember;
        _imgDisplay.sprite = _soMember._sprite;
        _txtTitle.text = _soMember._name;
        _txtType.text = _soMember.type.ToString();
        _txtDescription.text =  _soMember._description;
    }

    private void Start() {
        _bp.onClick.AddListener(UISelection);
    }

    private void UISelection() {
        OnSelection?.Invoke(this, EventArgs.Empty);
        StaticEvents.NewMemberSelected(_soMember);
    }
}