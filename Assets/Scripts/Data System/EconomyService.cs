using UnityEditor.UIElements;
using UnityEngine;

public static class EconomyService
{
    public static int Gold
    {
        get => SaveService.Current?.gold ?? 0;
        private set
        {
            if (SaveService.Current == null) return;
            SaveService.Current.gold = value;
            GameEvents.RaiseGoldChanged(value);
        }
    }

    public static bool TrySpendGold(int amount)
    {
        if(amount <= 0) return true;
        if(Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount; 
    }

    public static void ResetToDefault()
    {
        Gold = 100;
    }
}
