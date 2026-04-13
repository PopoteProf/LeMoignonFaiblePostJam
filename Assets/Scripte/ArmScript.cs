using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class ArmScript : MonoBehaviour, IMember {
    
    [SerializeField] private Animator _animator;
    [SerializeField] private Rig rig;
    [SerializeField] private BoxCollider2D DamageBounds;
    [SerializeField] private ContactFilter2D ContactFilter2D;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _prfPsDestroy;
    [SerializeField] private GameObject _prfPsPaf;
    [SerializeField] private AudioElement _aeAttack;

    public event EventHandler<IMember> OnMemberDestroy;
    
    public IMember.Membertype GetMenberType() => IMember.Membertype.Arm;
    public Transform GetTransform() => transform;
    private bool _shouldTargetPlayer;
    private int _damage;

    public void Attack(bool shouldTargetPlayer, int damage) {
        _aeAttack.PlayAsSFX();
        _shouldTargetPlayer = shouldTargetPlayer;
        _damage = damage;
        _animator.SetTrigger("Attack");
    }

    public void SetIsGrounded(bool isGrounded) {
        
    }

    public void DoDamage() {
        ColliderArray2D cols =DamageBounds.GetContactColliders(ContactFilter2D);
        foreach (var col in cols) {
            if (col.transform.GetComponent<Idamagable> ()!= null) {
                Idamagable idamagable = col.transform.GetComponent<Idamagable> ();
                if ((!_shouldTargetPlayer && idamagable.IsPlayer()) ) {
                    continue;
                }
                if((_shouldTargetPlayer && !idamagable.IsPlayer()))
                {
                    continue;
                }
                if( !_shouldTargetPlayer&&_impulseSource!=null)_impulseSource.GenerateImpulse();
                if(_prfPsPaf!=null)Instantiate(_prfPsPaf,col.transform.position,Quaternion.identity );
                col.transform.GetComponent<Idamagable> ().TakeDamage(_damage);
            }
        }
        //Physics2D.BoxCast()
    }


    public void Destroy()
    {
        Instantiate(_prfPsDestroy, DamageBounds.transform.position, Quaternion.identity);
        OnMemberDestroy?.Invoke(this,this);
        Destroy(gameObject);
    }

    public void SetRotation(float y) {
        transform.eulerAngles = new Vector3(0, 0, y);
    }
}