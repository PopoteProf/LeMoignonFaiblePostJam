using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour{
    
    [SerializeField] private Image _imgHPBar;
    [SerializeField] private EnnemiBasique _ennemiBasique;
    private void Awake() {
        _ennemiBasique.OnHPChange+= EnnemiBasiqueOnOnHPChange;
    }

    private void EnnemiBasiqueOnOnHPChange(object sender, float e) {
        _imgHPBar.fillAmount = e;
    }
}