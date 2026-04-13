using System;
using UnityEngine;


public interface IMember
{
    public enum Membertype {
        Leg, Arm
    }

    public event EventHandler<IMember> OnMemberDestroy;
    public Transform GetTransform();

    public abstract Membertype GetMenberType();
    public abstract void Attack(bool shouldTargetPlayer, int damage);
    public abstract void SetIsGrounded(bool isGrounded);
    public void Destroy();
    public void SetRotation(float y);

}