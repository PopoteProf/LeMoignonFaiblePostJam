using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{

    [SerializeField] private Transform _panelMainButton; 
    [SerializeField] private Button _bpPlay;
    [SerializeField] private Button _bpOptions;
    [SerializeField] private Button _bpCredit;
    [SerializeField] private Button _bpQuit;
    [SerializeField] private Button _bpCreditRetour;
    [SerializeField] private Transform _panelCredit;
    
    [Header("FeedBack")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _eye1;
    [SerializeField] private Transform _eye2;
    [SerializeField] private Transform _hand;
    [SerializeField] private float _handYMax;
    [SerializeField] private float _handYMin;
    [SerializeField] private AudioElement _aePanelOpen;

    private void Start() {
        _bpPlay.onClick.AddListener(UIButtonPlay);
        _bpOptions.onClick.AddListener(UIButtonOptions);
        _bpCredit.onClick.AddListener(UIButtonCredit);
        _bpQuit.onClick.AddListener(UIButtonQuit);
        _bpCreditRetour.onClick.AddListener(UIButtonCreditRetour);
    }
    

    private void Update() {
        
        _eye1.up = Mouse.current.position.ReadValue()-(Vector2)_eye1.position;
        _eye2.up = Mouse.current.position.ReadValue()-(Vector2)_eye2.position;
        Vector3 pos =_hand.position;
        pos.y = Mathf.Clamp(Mouse.current.position.y.value,_handYMin,_handYMax);
        _hand.transform.position = pos;
    }

    private void UIButtonPlay() {
        SceneManager.LoadScene(1);
    }

    private void UIButtonOptions()
    {
        
    }

    private void UIButtonCredit() {
        _panelMainButton.gameObject.SetActive(false);
        _panelCredit.gameObject.SetActive(true);
        AudioBus.OnPlayAudioElementSFX(_aePanelOpen);
    }

    private void UIButtonQuit()
    {
        Application.Quit();
    }

    private void UIButtonCreditRetour()
    {
        _panelMainButton.gameObject.SetActive(true);
        _panelCredit.gameObject.SetActive(false);
        AudioBus.OnPlayAudioElementSFX(_aePanelOpen);
    }
}
