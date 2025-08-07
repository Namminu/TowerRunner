using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public const int DefaultVersion = 1;

    public int version      = DefaultVersion;
    public int bestScore    = 0;
    public int gold         = 0;
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    public static GameData CreateDefault()
    {
        return new GameData
        {
            version = DefaultVersion,
            bestScore = 0,
            gold = 0,
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
