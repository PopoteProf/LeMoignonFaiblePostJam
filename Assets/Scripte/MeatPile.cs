using UnityEngine;

public class MeatPile : MonoBehaviour {
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private SoMemberList _soMemberList;
    public void Start() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, _groundLayer);
        transform.position = hit.point;
        transform.up = hit.normal;
    }

    public void SetSoMemberList(SoMemberList m) {
        _soMemberList = m;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerController>()) {
            Debug.Log("GetMeatPile");       
            StaticEvents.MeatPileEnter(_soMemberList);
            Destroy(gameObject);
        }
    }
}