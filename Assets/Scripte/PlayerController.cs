using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, Idamagable
{

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxVelocity =10;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundDistancecheck = 1;
    [SerializeField] private float _distanceToGround =0.5f;
    [SerializeField] private float _bodyRotationStep;
    [SerializeField] private float _bodypositionStep;
    [Space(10), Header("Jump")]
    [SerializeField] private float _jumpPower =3;
    [SerializeField] private float _gravity =10;
    [Space(10), Header("BodyParts")]
    [SerializeField] private MemberSocket[] _legSockets;
    [SerializeField] private MemberSocket[] _ArmSockets;
    [Space(5)] 
    [SerializeField] private ArmScript _prfArm;
    [SerializeField] private FeetScript _prfLeg;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private AudioElement _aeMemberAdded;
    [SerializeField] private AudioElement _aeMemberRemove;
    [SerializeField] private AudioElement _aeAttack;
    [SerializeField] private AudioElement _aeJump;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _jump;
    
    Vector2 _moveDirection;
    private float _yVelocity;
    private bool _isJumping;
    private bool _isGrounded;
    private int _memberCount = 0;

    private void Awake() {
        foreach (var soket in _legSockets) soket.OnMemberAutoAdd+= SocketAutoAddMenber;
        foreach (var soket in _ArmSockets) soket.OnMemberAutoAdd+= SocketAutoAddMenber;
    }

    private void Start() {
        _moveAction =InputSystem.actions.FindAction("Move");
        _moveAction.performed+= MoveActionOnperformed;
        _moveAction.canceled += MoveActionOnperformed;
        _attackAction = InputSystem.actions.FindAction("Attack");
        _attackAction.performed += AttackActionOnperformed;
        _jump = InputSystem.actions.FindAction("Jump");
        _jump.started += JumpOnstarted;
        
        StaticEvents.OnNewMemberSelected+= StaticEventsOnOnNewMemberSelected;
    }

    private void OnDestroy()
    {
        StaticEvents.OnNewMemberSelected-= StaticEventsOnOnNewMemberSelected;
        _moveAction.performed-= MoveActionOnperformed;
        _moveAction.canceled -= MoveActionOnperformed;
        _attackAction.performed -= AttackActionOnperformed;
        _jump.started -= JumpOnstarted;
        }

    private void StaticEventsOnOnNewMemberSelected(object sender, SoMember e) {
        AddNewMember(e);
        
    }

    private void SocketAutoAddMenber(object sender, IMember e) {
        MemberSocket socket = (sender as MemberSocket);
        socket.SetCurrentMember(e);
        if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
    }

    private void JumpOnstarted(InputAction.CallbackContext obj) {
        //_yVelocity = _jumpPower;
        if (!_isGrounded) return;
        AudioBus.OnPlayAudioElementSFX(_aeJump);
        _rb.AddForceY(_jumpPower,  ForceMode2D.Impulse);
        _isGrounded = false;
        _isJumping = true;
        foreach (var socket in _legSockets) {
            if (!socket.IsSocketFree) {
                socket.GetMember.SetIsGrounded(false);
            }
        }
    }

    private void AttackActionOnperformed(InputAction.CallbackContext obj) {
        AudioBus.OnPlayAudioElementSFX(_aeAttack);
        foreach (var socket in _ArmSockets) {
            if (!socket.IsSocketFree) {
                socket.Attack(false, 1);
            }
        }
    }

    private void MoveActionOnperformed(InputAction.CallbackContext obj) {
        _moveDirection = obj.action.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update() {
        ManageMouvement();
        ManageGroundContact();
        if(!_isGrounded)ManageGravity();
    }

    private void ManageGravity()
    {
        _yVelocity-= _gravity*Time.deltaTime;
    }

    private void ManageMouvement() {
        
        _rb.AddForce(transform.right* (_moveDirection.x* _moveSpeed * Time.deltaTime));
        if (Mathf.Abs(_rb.linearVelocityX) > _maxVelocity) {
            _rb.linearVelocityX =Mathf.Clamp(_rb.linearVelocityX,-_maxVelocity, _maxVelocity);
        }
    }

    private void ManageGroundContact() {
        if (_isJumping && _rb.linearVelocityY > 0) {
           // _characterController.Move(new Vector2(0,_yVelocity) * Time.deltaTime);
            transform.up = Vector3.RotateTowards(transform.up, Vector3.up, _bodyRotationStep*Time.deltaTime,100 );
            return;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, _groundDistancecheck, _groundLayerMask);
        if (hit ) {
            if (!_isGrounded) {
                _isGrounded = true;
                _yVelocity = 0;
                foreach (var socket in _legSockets) {
                    if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
                }
            }
            Debug.DrawLine(transform.position,hit.point, Color.green);

            _rb.linearVelocityY = -0.2f;
            transform.position =Vector3.MoveTowards(transform.position,hit.point+Vector2.up * _distanceToGround,_bodypositionStep*Time.deltaTime) ;
            transform.up = Vector3.RotateTowards(transform.up, hit.normal, _bodyRotationStep*Time.deltaTime,100 );
        }
        else {
            _isGrounded = false;
            //_characterController.Move(new Vector2(0,_yVelocity) * Time.deltaTime);
            transform.up = Vector3.RotateTowards(transform.up, Vector3.up, _bodyRotationStep*Time.deltaTime,100 );
            Debug.DrawRay(transform.position, -transform.up * _groundDistancecheck, Color.red);
        }
    }

    [ContextMenu("AddRandomMember")]
    private void AddRandomMember() {
        if (Random.Range(0, 2) < 1) {
            List<MemberSocket> sockets = GetAllFreeMemberOfType(IMember.Membertype.Leg);
            if (sockets == null || sockets.Count == 0) return; 
            MemberSocket socket = sockets[Random.Range(0, sockets.Count)];
            FeetScript leg = Instantiate(_prfLeg, socket.Position, socket.Rotation);
            leg.transform.SetParent(socket.transform);
            socket.SetCurrentMember(leg);
            if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
            
        }
        else {
            Debug.Log("Addarm");
            List<MemberSocket> sockets = GetAllFreeMemberOfType(IMember.Membertype.Arm);
            if (sockets == null || sockets.Count == 0) return; 
            MemberSocket socket = sockets[Random.Range(0, sockets.Count)]; 
            ArmScript arm = Instantiate(_prfArm, socket.Position, socket.Rotation);
            arm.transform.SetParent(socket.transform);
            socket.SetCurrentMember(arm);
            if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
        }
    }

    private void AddNewMember(SoMember soMember) {
        MemberSocket socket = GetSocketToSpawnMember(soMember.type);
        IMember m = Instantiate(soMember._prfMember, socket.Position, socket.Rotation).GetComponent<IMember>();
        m.GetTransform().SetParent(socket.transform);
        socket.SetCurrentMember(m);
        if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
        AudioBus.OnPlayAudioElementSFX(_aeMemberAdded);
    }

    private MemberSocket GetSocketToSpawnMember(IMember.Membertype type) {
        List<MemberSocket> sockets ;
        switch (type)
        {
            case IMember.Membertype.Leg:sockets= GetAllFreeMemberOfType(IMember.Membertype.Leg); break;
            case IMember.Membertype.Arm:sockets= GetAllFreeMemberOfType(IMember.Membertype.Arm); break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        MemberSocket socket ;
        if (sockets == null || sockets.Count == 0) {
            socket = _legSockets[Random.Range(0, _legSockets.Length)];
            socket.GetMember.Destroy();
        }
        else {
            socket = sockets[Random.Range(0, sockets.Count)];
        }
        return socket;
    }

    [ContextMenu("RemoveRandomMember")]
    private void RemoveRandomMember()
    {
        List<IMember> members = GetAllUsedMembers();
        members[Random.Range(0, members.Count)].Destroy();
        AudioBus.OnPlayAudioElementSFX(_aeMemberRemove);
        _memberCount--;
    }

    private List<MemberSocket> GetAllFreeMemberOfType(IMember.Membertype type) {
        MemberSocket[] sockets;
        switch (type) {
            case IMember.Membertype.Leg: sockets = _legSockets; break;
            case IMember.Membertype.Arm: sockets = _ArmSockets; break;
            default: return null;
        }
        List<MemberSocket> freeSockets = new List<MemberSocket>();
        foreach (var socket in sockets) {
            if( socket.IsSocketFree) freeSockets.Add(socket);
        }
        Debug.Log(freeSockets.Count);
        return freeSockets;
    }
    
    private List<IMember> GetAllUsedMembers() {
        List<IMember> members = new List<IMember>();
        foreach (var socket in _ArmSockets) {
            if (!socket.IsSocketFree) {
                members.Add(socket.GetMember);
            }
        }
        foreach (var socket in _legSockets) {
            if (!socket.IsSocketFree) {
                members.Add(socket.GetMember);
            }
        }

        return members;
    }

    public bool IsPlayer() => true;

    public void TakeDamage(int damage) {
        RemoveRandomMember();
        if (_impulseSource!=null) _impulseSource.GenerateImpulse();
        if (CountMembers() <= 0) {
            Destroy(gameObject);
            StaticEvents.GameOver();
        }
        
    }

    private int CountMembers() {
        int count = 0;
        foreach (var socket in _ArmSockets) if (!socket.IsSocketFree) count++;
        foreach (var socket in _legSockets) if (!socket.IsSocketFree) count++;
        return count;
    }
}