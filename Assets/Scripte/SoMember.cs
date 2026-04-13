using UnityEngine;

[CreateAssetMenu(fileName = "SoMember", menuName =  "SO/SoMember")]
public class SoMember : ScriptableObject {
    public GameObject _prfMember;
    public IMember.Membertype type;
    public string _name;
    [TextArea]public string _description;
    public Sprite _sprite;
}