using UnityEngine;

public class EndGameTriggerZone : MonoBehaviour
{
    [SerializeField] private bool _isWin;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.GetComponent<PlayerController>()!=null) {
            if (_isWin)StaticEvents.Win();
            else StaticEvents.GameOver();
        }
    }
}