using System;
using UnityEngine;
using UnityEngine.U2D;

public class TestiPulse : MonoBehaviour ,Idamagable
{
    [SerializeField] private int _hp = 3;
    [SerializeField] private SpriteRenderer _meshRenderer;
    [SerializeField] private float _damageTime=  0.5f;
    [SerializeField] private AnimationCurve _damageCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField,Tooltip("Spawn this prefabs when destroy")] private GameObject _prfTagertDestroy;
    [SerializeField] private SpriteShapeRenderer _nerve;
    [SerializeField] private GameObject _sphincter;
    [SerializeField] private AudioElement _aeDamaged;
    [SerializeField] private AudioElement _aeDestroyed;
    [SerializeField] private GameObject _prfPsDestroy;
    public bool IsPlayer() => false;
    
    private PopoteTimer _damagedTimer;

    private void Start() {
        _damagedTimer = new PopoteTimer(_damageTime);
        _damagedTimer.OnTimerEnd += OnTimerEnd;
    }
    
    private void OnTimerEnd(object sender, EventArgs e)
    {
        _meshRenderer.material.SetFloat("_HitProgress", 0);
    }

    public void TakeDamage(int damage)
    {
        _damagedTimer.Play();
        //SpawnHit(hitPoint, hitNormal);
        _hp-=damage;
        if (_hp <= 0) {
            SpawnDeath();
            _sphincter.gameObject.SetActive(false);
            foreach (var mat in  _nerve.materials)
            {
                mat.SetFloat("_DeformPower", 0);
            }

            //_nerve.material
            Instantiate(_prfPsDestroy,transform.position,Quaternion.identity);
            _aeDestroyed.PlayAsSFX();
            Destroy(gameObject);
        }
        _aeDamaged.PlayAsSFX();
    }
    private void SpawnDeath() {
        if (_prfTagertDestroy == null) return;
        GameObject go = Instantiate(_prfTagertDestroy, transform.position, Quaternion.identity);
    }
    private void Update() {
        _damagedTimer.UpdateTimer();
        if (_damagedTimer.IsPlaying) {
            ManagerDamaged();
            return;
        }
    }
    private void ManagerDamaged() {
        _meshRenderer.material.SetFloat("_HitProgress", _damageCurve.Evaluate(_damagedTimer.T));
    }
}