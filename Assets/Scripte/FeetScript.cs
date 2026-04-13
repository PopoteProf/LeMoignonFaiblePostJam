using System;
using UnityEngine;

public class FeetScript : MonoBehaviour , IMember
{
    [SerializeField] private Transform _raycaster;
    [SerializeField] private Transform _footTarget;
    [SerializeField] private Transform _footJumpTarget;
    [SerializeField] private float _distanceToMove;
    [SerializeField] private float _animationSpeed =1;
    [SerializeField] private float _aniamtionYoffset =2;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Offsets")] 
    [SerializeField] private Vector2 _feetPosOffset;
    [SerializeField] private Vector2 _feetDirOffset;
    
    private Vector2 _currentFeetPosition;
    private Vector2 _currentFeetDirection;
    private PopoteTimer _animationTimer;
    
    private Vector2 _oldPosFeetPosition;
    private Vector2 _oldFeetDirection;
    private Vector2 _newFeetPosition;
    private Vector2 _newFeetDirection;
    [SerializeField]private bool _isGrounded;
    [SerializeField] private AudioElement _aeFeetOnGround;
    [SerializeField] private GameObject _prfPsDestroy;

    public event EventHandler<IMember> OnMemberDestroy;

    public Transform GetTransform() => transform;
    public IMember.Membertype GetMenberType()=>  IMember.Membertype.Leg;
    public void Attack(bool shouldTargetPlayer, int damage) {}
    
    
    public void SetIsGrounded(bool isGrounded) {
        _isGrounded = isGrounded;
        if (!isGrounded) {
            StartLegMovement(_footJumpTarget.position, _footJumpTarget.right);
        }
        else {
            CheckGround(true);
        }
    }

    

    public void Destroy() {
        Instantiate(_prfPsDestroy, transform.position, Quaternion.identity);
        OnMemberDestroy?.Invoke(this,this);
        Destroy(gameObject);
    }

    public void SetRotation(float y) {
    }

    private void Awake()
    {
        _animationTimer = new PopoteTimer(_animationSpeed);
        _animationTimer.OnTimerEnd += OnTimerEnd;
    }

    void Start() {
        RaycastHit2D hit = Physics2D.Raycast(_raycaster.position, _raycaster.up,Mathf.Infinity, _groundLayerMask);
        
        _currentFeetDirection = hit.normal;
        _currentFeetPosition = hit.point;
        _footTarget.position = _currentFeetPosition;
        _footTarget.up =  _currentFeetDirection;
        
    }
    
    void Update()
    {
        _animationTimer.UpdateTimer();
        if (_animationTimer.IsPlaying) {
            ManagerAnimation();
        }
        else {
            if (!_isGrounded) {
                _currentFeetPosition = _footJumpTarget.position;
                _currentFeetDirection = _footJumpTarget.right;
            }
            _footTarget.position = _currentFeetPosition + _feetPosOffset;
            _footTarget.right = -_currentFeetDirection + _feetDirOffset;
           if(_isGrounded)CheckGround(false);
        }
    }

    private void CheckGround(bool forceToUpdate)
    {
        RaycastHit2D hit = Physics2D.Raycast(_raycaster.position, _raycaster.up ,Mathf.Infinity,_groundLayerMask);
        Debug.DrawLine(_raycaster.position, hit.point, Color.red);
        if (Vector2.Distance(hit.point, _currentFeetPosition) > _distanceToMove||forceToUpdate) {
            StartLegMovement(hit.point, hit.normal);
        }
    }

    private void StartLegMovement(Vector2 pos , Vector2 dir) {
        _animationTimer.Play();
        _oldFeetDirection = _currentFeetDirection;
        _oldPosFeetPosition = _currentFeetPosition;
        _newFeetDirection = dir;
        _newFeetPosition = pos;
    }

    private void ManagerAnimation() {
        if (!_isGrounded) {
            _newFeetPosition = _footJumpTarget.position;
            _newFeetDirection = _footJumpTarget.right;
        }
        Vector2 c = Vector2.Lerp(_oldPosFeetPosition, _newFeetPosition, 0.5f)+new Vector2(0, _aniamtionYoffset);
        Vector2 ac = Vector2.Lerp(_oldPosFeetPosition, c, _animationTimer.T);
        Vector2 cb = Vector2.Lerp(c, _newFeetPosition, _animationTimer.T);
        _currentFeetPosition = Vector2.Lerp(ac, cb, _animationTimer.T);
        _footTarget.position = _currentFeetPosition+_feetPosOffset;
        _currentFeetDirection =-Vector2.Lerp(_oldFeetDirection, _newFeetDirection, _animationTimer.T);
        _footTarget.right = _currentFeetDirection + _feetDirOffset;
    }
    
    
    private void OnTimerEnd(object sender, EventArgs e) {
        _currentFeetPosition = _newFeetPosition;
        _currentFeetDirection = _newFeetDirection;
        if(_spriteRenderer.isVisible && _aeFeetOnGround!=null) AudioBus.OnPlayAudioElementSFX(_aeFeetOnGround);
    }
}