using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public const int DefaultVersion = 1;

    public int version      = DefaultVersion;
    public int bestScore    = 0;
    public int gold         = 100;
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    public static GameData CreateDefault()
    {
        return new GameData
        {
            version = DefaultVersion,
            bestScore = 0,
            gold = 100,
            upgrades = new List<UpgradeData>()
        };
    }
}

[Serializable]
public class UpgradeData
{
    public string   id;
    public int      level;
}
