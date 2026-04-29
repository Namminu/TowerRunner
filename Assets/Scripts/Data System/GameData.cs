using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public const int DefaultVersion = 1;

    public int version      = DefaultVersion;
    public int bestScore    = 0;
    public int gold         = 0;
    public List<UpgradeData> upgrades = new List<UpgradeData>();
    public int[] inventory = new int[3] { -1, -1, -1};

    public static GameData CreateDefault()
    {
        return new GameData
        {
            version = DefaultVersion,
            bestScore = 0,
            gold = 1000000,
            upgrades = new List<UpgradeData>(),
            inventory = new int[3] { -1, -1, -1}
        };
    }
}

[Serializable]
public class UpgradeData
{
    public string   id;
    public int      level;
}
