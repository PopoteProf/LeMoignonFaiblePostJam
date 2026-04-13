using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonFeedBack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private bool _playSounds = true;
    [SerializeField] private AudioElement _acPointerEntrer;
    [SerializeField] private AudioElement _acPointerExit;
    [SerializeField] private AudioElement _acPointerDown;

    [SerializeField] private bool _playTweening = true;
    [SerializeField] private float _animationTime = 0.3f;
    [SerializeField] private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] private float _animationEndScale = 1.2f;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_playSounds)
        {
            AudioBus.OnPlayAudioElementSFX(_acPointerEntrer);
        }

        if (_playTweening) {
            transform.DOPause();
            transform.DOScale(_animationEndScale, _animationTime).SetEase(_animationCurve);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_playSounds) {
           // AudioBus.OnPlayAudioElementSFX(_acPointerExit);
           
        }
        if (_playTweening) {
            transform.DOPause();
            transform.DOScale(1, _animationTime).SetEase(_animationCurve);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_playSounds) {
            AudioBus.OnPlayAudioElementSFX(_acPointerDown);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnPointerEnter(null);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnPointerExit(null);
    }
}