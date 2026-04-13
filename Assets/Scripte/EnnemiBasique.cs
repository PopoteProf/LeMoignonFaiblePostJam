using System;
using UnityEngine;

public class EnnemiBasique : MonoBehaviour , Idamagable
{
    public event EventHandler<float> OnHPChange; 
    public enum EnnemiState {
        Waiting, Walking, Attacking
    }
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private EnnemiState _ennemiState;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxVelocity =10;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundDistancecheck = 1;
    [SerializeField] private float _distanceToGround =0.5f;
    [SerializeField] private float _bodyRotationStep;
    [SerializeField] private float _bodypositionStep;
    [SerializeField] private int _maxHP = 5;
    [SerializeField] private MeatPile _prfMeatPile;
    [SerializeField] private SoMemberList _soMemberList;
    
    [Space(10), Header("BodyParts")]
    [SerializeField] private MemberSocket[] _legSockets;
    [SerializeField] private MemberSocket[] _ArmSockets;
    [Space(10), Header("Sensor")]
    [SerializeField] private Transform _leftRayCaster;
    [SerializeField] private Transform _rightRayCaster;
    [SerializeField] private float _lateralRayCastDistance;
    [SerializeField] private float _downRayCastDistance;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private float _attackTime = 2f;
    [SerializeField] private float _damageTime=  0.5f;
    [SerializeField] private AnimationCurve _damageCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] private AudioElement _aeHit;
    [SerializeField] private AudioElement _aeAttack;

    
    private bool _isGrounded;
    [SerializeField]private Vector2 _moveDirection;
    private float _yVelocity;
    private int _currentHP;

    private PopoteTimer _timerwait;
    private PopoteTimer _timerattack;
    private PopoteTimer _damagedTimer;
    
    private void Awake() {
        foreach (var soket in _legSockets) soket.OnMemberAutoAdd+= SocketAutoAddMenber;
        foreach (var soket in _ArmSockets) soket.OnMemberAutoAdd+= SocketAutoAddMenber;
        
    }

    private void Start() {
        _timerwait = new PopoteTimer(_waitTime);
        _timerattack = new PopoteTimer(_attackTime);
        _timerwait.OnTimerEnd+= OnWaitTimerEnd;
        _timerattack.OnTimerEnd+= OnAttackTimerEnd;
        _damagedTimer = new PopoteTimer(_damageTime);
        _damagedTimer.OnTimerEnd += OnTimerEnd;
        _currentHP = _maxHP;
        StartWait();
    }
    private void OnTimerEnd(object sender, EventArgs e)
    {
        _spriteRenderer.material.SetFloat("_HitProgress", 0);
    }

    private void OnAttackTimerEnd(object sender, EventArgs e)
    {
        StartWait();
    }

    private void OnWaitTimerEnd(object sender, EventArgs e) {
        _ennemiState = EnnemiState.Walking;
    }

    private void SocketAutoAddMenber(object sender, IMember e) {
        MemberSocket socket = (sender as MemberSocket);
        socket.SetCurrentMember(e);
        if (!socket.IsSocketFree) socket.GetMember.SetIsGrounded(true);
    }


    private void Update()
    {
        ManageGroundContact();
        ManageMouvement();
        ManageSensor();
        _timerwait.UpdateTimer();
        _timerattack.UpdateTimer();
        

    
        _damagedTimer.UpdateTimer();
        if (_damagedTimer.IsPlaying) {
        ManagerDamaged();
        return;
        }
    }
    
        private void ManagerDamaged() {
            _spriteRenderer.material.SetFloat("_HitProgress", _damageCurve.Evaluate(_damagedTimer.T));
        }
    

    private void ManageMouvement() {
        
        if( _ennemiState == EnnemiState.Walking) _rb.AddForce(transform.right* (_moveDirection.x* _moveSpeed * Time.deltaTime));
        
        if (Mathf.Abs(_rb.linearVelocityX) > _maxVelocity) {
            _rb.linearVelocityX =Mathf.Clamp(_rb.linearVelocityX,-_maxVelocity, _maxVelocity);
        }
    }
    
    private void ManageGroundContact() {
        if ( _rb.linearVelocityY > 0) {
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
            transform.up = Vector3.RotateTowards(transform.up, Vector3.up, _bodyRotationStep*Time.deltaTime,100 );
            Debug.DrawRay(transform.position, -transform.up * _groundDistancecheck, Color.red);
        }
    }

    private void ManageSensor()
    {
        if (_ennemiState == EnnemiState.Attacking) return;
        RaycastHit2D hit;
        if (_moveDirection.x > 0) {
            hit = Physics2D.Raycast(_rightRayCaster.position, _rightRayCaster.right, _lateralRayCastDistance);
            if (hit.collider != null && hit.collider.GetComponent<PlayerController>()) {
                StartAttack();
            }
        }
        
        if (_timerwait.IsPlaying)return;

        if (_moveDirection.x > 0) {
            hit = Physics2D.Raycast(_rightRayCaster.position, _rightRayCaster.right, _lateralRayCastDistance, _groundLayerMask);
            
            if (hit.collider!=null) {
                StartWait();
                _moveDirection = new Vector2(-1, 0);
                return;
            }
            hit = Physics2D.Raycast(_rightRayCaster.position, -_rightRayCaster.up, _downRayCastDistance, _groundLayerMask);
            if (hit.collider == null) {
                StartWait();
                _moveDirection = new Vector2(-1, 0);
                return;
            }
        }
        if (_moveDirection.x < 0) {
            hit = Physics2D.Raycast(_leftRayCaster.position, -_leftRayCaster.right, _lateralRayCastDistance, _groundLayerMask);
            if (hit.collider != null) {
                StartWait();
                _moveDirection = new Vector2(1, 0);
                return;
            }
            hit = Physics2D.Raycast(_leftRayCaster.position, -_leftRayCaster.up, _downRayCastDistance, _groundLayerMask);
            if (hit.collider == null) {
                StartWait();
                _moveDirection = new Vector2(1, 0);
                return;
            }
        }
    }

    private void StartAttack() {
        _timerwait.Pause();
        _timerattack.Play();
        _ennemiState = EnnemiState.Attacking;
        _aeAttack.PlayAsSFX();

        foreach (var socket in _ArmSockets) {
            if( !socket.IsSocketFree) socket.Attack(true, 1);
        }
    }

    private void StartWait() {
        
        _timerwait.Play();
        _ennemiState = EnnemiState.Waiting;
    }

    public bool IsPlayer() => false;

    public void TakeDamage(int damage) { 
        
        _currentHP  -= damage;
        OnHPChange?.Invoke(this,(float)_currentHP/_maxHP);
        _aeHit.PlayAsSFX();
        if (_currentHP <= 0) {
            MeatPile m =Instantiate(_prfMeatPile, transform.position, Quaternion.identity);
            m.SetSoMemberList(_soMemberList);
            Destroy(gameObject);
        }
        _damagedTimer.Play();
    }
}