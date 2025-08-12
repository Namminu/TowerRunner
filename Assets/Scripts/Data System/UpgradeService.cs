using UnityEngine;

public static class UpgradeService
{
    private static void EnsureIndex(int idx)
    {
        var list = SaveService.Current.upgrades;
        while(list.Count <= idx)
        {
            list.Add(new UpgradeData
            {
                id = $"U{list.Count}",
                level = EnforceService.GetInitLevel(list.Count)
            });
        }
    }

    public static int GetLevel(int idx)
    {
        EnsureIndex(idx);
        return SaveService.Current.upgrades[idx].level;
    }

    public static void SetLevel(int idx, int level, bool applyToPlayer = true)
    {
        EnsureIndex(idx);
        SaveService.Current.upgrades[idx].level = level;

        if(applyToPlayer && Player.Instance != null)
        {
            float value = EnforceService.GetValue(idx, level);
            Player.Instance.ApplyUpgrade((EnforceType)idx, value);
        }
    }
}