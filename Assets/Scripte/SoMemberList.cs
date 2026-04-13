using UnityEngine;

[CreateAssetMenu(fileName = "SoMemberList", menuName =  "SO/SoMemberList")]
public class SoMemberList : ScriptableObject {
    public SoMember[] rewards;

    public SoMember GetReward() {
        return rewards[Random.Range(0, rewards.Length)];
    }
}