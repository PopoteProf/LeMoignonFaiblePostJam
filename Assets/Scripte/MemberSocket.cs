using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MemberSocket : MonoBehaviour {
    public event EventHandler<IMember> OnMemberAutoAdd; 
    
    [SerializeField]private IMember.Membertype MemberType;
    [SerializeField]private UISocketMember _uiSocketMember;
    public bool IsSocketFree => _currentMember == null;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public IMember GetMember => _currentMember;

    private IMember _currentMember;
    
    private InputAction _crouchAction;
    private bool _uiOpen;
    
    public void SetCurrentMember(IMember member) {
        _currentMember = member;
        _currentMember.OnMemberDestroy += CurrentMemberOnOnMemberDestroy;
    }

    private void CurrentMemberOnOnMemberDestroy(object sender, IMember e) {
        _currentMember = null;
        
    }

    public void Attack(bool shouldTargetPlayer, int damage) {
        if (_currentMember != null) _currentMember.Attack(shouldTargetPlayer, damage);
    }
    
    public void Start()
    {
        _crouchAction =InputSystem.actions.FindAction("Crouch");
        _crouchAction.started+= CrouchActionOnstarted;
        if(_uiSocketMember != null)_uiSocketMember.OnMemberRotChange+= UiSocketMemberOnOnMemberRotChange;
        if (transform.childCount>0&&transform.GetChild(0).GetComponent<IMember>() != null) {
            OnMemberAutoAdd?.Invoke(this, transform.GetChild(0).GetComponent<IMember>());
        }
    }

    private void UiSocketMemberOnOnMemberRotChange(object sender, float e) {
        _currentMember.SetRotation(e);
    }

    private void CrouchActionOnstarted(InputAction.CallbackContext obj) {

        if (_uiSocketMember == null|| _currentMember==null|| MemberType!= IMember.Membertype.Arm) return;
        if (!_uiOpen) {
            _uiSocketMember.gameObject.SetActive(true);
            _uiOpen = true;
        }
        else {
            _uiSocketMember.gameObject.SetActive(false);
            _uiOpen = false;
        }
        
    }
}