using System;
using UnityEngine;

public static class StaticEvents
{

   
    public static event EventHandler<SoMemberList> OnMeatPileEnter;
    public static event EventHandler<SoMember> OnNewMemberSelected;
    public static event EventHandler OnGameOver;
    public static event EventHandler OnWin;
    
    public static void NewMemberSelected(SoMember m) {
        OnNewMemberSelected?.Invoke(null, m);
    }

    public static void MeatPileEnter(SoMemberList m) {
        OnMeatPileEnter?.Invoke(null, m);
    }

    public static void GameOver()
    {
        OnGameOver?.Invoke(null, null);
    }

    public static void Win() {
        OnWin?.Invoke(null, null);
    }

    
}