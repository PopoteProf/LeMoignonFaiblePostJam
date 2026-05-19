using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour {
    [SerializeField] private CinemachinePositionComposer _cameraBase;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private float _zoomTime = 0.5f;
    [SerializeField] private AnimationCurve _zoomcurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Header("Zoom out")]
    [SerializeField]private float _zoomOutLens =10;
    [SerializeField]private Vector2 _zoomOutOffset = new Vector2(-3, 2);
    
    [Header("Zoom in")]
    [SerializeField]private float _zoomInLens = 4;
    [SerializeField]private Vector2 _zoomInOffset = new Vector2(0, 0);
    
    private PopoteTimer _timer;
    private bool _isZoomout;
    
    private float _beginZoom = 4;
    private Vector2 _beginOffset = new Vector2(0, 0);

    private void Start() {
        _timer = new PopoteTimer(_zoomTime);
        StaticEvents.OnConfiguring+= StaticEventsOnOnConfiguring;
    }

    private void StaticEventsOnOnConfiguring(object sender, bool e)
    {
        if (e)  DoZoomIn();
        else DoZoomOut();
    }

    private void Update() {
        _timer.UpdateTimer();
        if(_timer.IsPlaying) ManageZoom();}

    private void ManageZoom()
    {
        float t = _zoomcurve.Evaluate(_timer.T);
        if (_isZoomout) {
            _camera.Lens.OrthographicSize=  Mathf.Lerp(_beginZoom, _zoomOutLens, t);
            _cameraBase.TargetOffset =  Vector3.Lerp(_beginOffset, _zoomOutOffset, t);
        }
        else
        {
            _camera.Lens.OrthographicSize =  Mathf.Lerp(_beginZoom, _zoomInLens, t);
            _cameraBase.TargetOffset =  Vector3.Lerp(_beginOffset, _zoomInOffset, t);
        }
    }

    [ContextMenu("DoZoomOut")]
    private void DoZoomOut()
    {
        _isZoomout = true;
        _beginOffset = _cameraBase.TargetOffset;
        _beginZoom = _camera.Lens.OrthographicSize;
        _timer.Play();

    }

    [ContextMenu("DoZoomIn")]
    private void DoZoomIn() {
        _isZoomout = false;
        _beginOffset = _cameraBase.TargetOffset;
        _beginZoom = _camera.Lens.OrthographicSize;
        _timer.Play();
    }
}