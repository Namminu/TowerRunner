using UnityEngine;

public static class EnforceService
{
    private static EnforceDatabase _db;

    public static void Initialize(EnforceDatabase db)
    {
        _db = db;
    }

	/// <summary>
	/// Calculate actual numerical value based on current level
	/// </summary>
	public static float GetValue(int idx, int level)
    {
        var data = _db.enforceDB[idx];
        return data.initValue + data.valueIncrement * (level - data.initLevel);
    }

	/// <summary>
	/// Calculate the cost of upgrading from your current level to the next level
	/// </summary>
	public static int GetCost(int idx, int level)
    {
        var d = _db.enforceDB[idx];
        float cost = d.initCost * Mathf.Pow(d.costMultiplier, level - d.initLevel);
        return Mathf.CeilToInt(cost);
    }

    public static int GetInitLevel(int idx)
    {
        return _db.enforceDB[idx].initLevel;
    }

    public static EnforceDatabase.EnforceData GetData(int idx)
    {
        return _db.enforceDB[idx];
    }
}
