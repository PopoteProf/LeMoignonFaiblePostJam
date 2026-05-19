using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SoMember", menuName =  "SO/SoMember")]
public class SoMember : ScriptableObject {
    public GameObject _prfMember;
    public IMember.Membertype type;
    public string _name;
    public LocalizedString _LocalizerdName;
    public LocalizedString _LocalizerdDescription;
    public LocalizedString _LocalizerdType;
    [TextArea]public string _description;
    public Sprite _sprite;
}