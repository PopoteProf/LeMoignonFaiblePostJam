using System;
using UnityEngine;

public class FireTarget : MonoBehaviour, Idamagable
{

    [SerializeField] private int _hp;
    [SerializeField] private bool _canBeDestroy;
    
    [SerializeField] private SpriteRenderer _meshRenderer;
    [SerializeField] private float _damageTime=  0.5f;
    [SerializeField] private AnimationCurve _damageCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Space(10)] 
    [SerializeField,Tooltip("Spawn This Prefab When hit;")] private GameObject _prfTagertHit;
    [SerializeField,Tooltip("Spawn  the hit prefab on hit point or on the center of the target")] private bool _spwanHitOnHitPoint;
    [SerializeField, Tooltip("oriente the hit prefab UP transform in the direction of the hit normal ")] private bool _spwanHitOnHitNormal;
    [SerializeField,Tooltip("Spawn this prefabs when destroy")] private GameObject _prfTagertDestroy;
    [SerializeField,Tooltip("Spawn  the Death prefab on hit point or on the center of the target")] private bool _spwanDestroyOnHitPoint;
    [SerializeField, Tooltip("oriente the Death prefab UP transform in the direction of the hit normal ")] private bool _spwanDestroyOnHitNormal;

    private PopoteTimer _damagedTimer;

    private void Start()
    {
        _damagedTimer = new PopoteTimer(_damageTime);
        _damagedTimer.OnTimerEnd += OnTimerEnd;

    }

    private void OnTimerEnd(object sender, EventArgs e)
    {
        _meshRenderer.material.SetFloat("_HitProgress", 0);
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
    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) { 
        _damagedTimer.Play();
        SpawnHit(hitPoint, hitNormal);
        if (_canBeDestroy) _hp--;
        if (_hp <= 0) {
            SpawnDeath(hitPoint, hitNormal);
            Destroy(gameObject);
        }
    }

    private void SpawnHit(Vector3 hitPoint, Vector3 hitNormal) {
        if (_prfTagertHit == null) return;
        GameObject go;
        if (_spwanHitOnHitPoint) go = Instantiate(_prfTagertHit, hitPoint, Quaternion.identity);
        else go = Instantiate(_prfTagertHit, transform.position, Quaternion.identity);
        
        if( _spwanHitOnHitNormal) go.transform.up = hitNormal;
    }
    private void SpawnDeath(Vector3 hitPoint, Vector3 hitNormal) {
        if (_prfTagertDestroy == null) return;
        GameObject go;
        if (_spwanDestroyOnHitPoint) go = Instantiate(_prfTagertDestroy, hitPoint, Quaternion.identity);
        else go = Instantiate(_prfTagertDestroy, transform.position, Quaternion.identity);
        
        if( _spwanDestroyOnHitNormal) go.transform.up = hitNormal;
    }

    public bool IsPlayer() {
        return false;
    }

    public void TakeDamage(int damage) {
        _damagedTimer.Play();
    }
}