using System;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveService
{
    public static GameData Current { get; private set; }

	/// <summary>
	/// Load all game data when starting the game
	/// </summary>
	public static async Task InitializeAsync()
    {
        try
        {
            Current = await SaveSystem.LoadAsync();
            if (Current == null)
            {
                Debug.LogWarning("SaveService.InitializeAsync: loaded GameData is null. Using default data.");
                Current = GameData.CreateDefault();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveService.InitializeAsync failed: {ex}");
            Current = GameData.CreateDefault();
        }
    }

	/// <summary>
	/// Save all game data recorded to date
	/// </summary>
	public static async Task SaveAllAsync()
    {
        Prefs.Save();

        if (Current == null)
        {
            Debug.LogWarning("SaveService.SaveAllAsync skipped: Current GameData is null.");
            return;
        }

        await SaveSystem.SaveAsync(Current);
    }

	/// <summary>
	/// Delete game progress data
	/// </summary>
	public static void ResetSaveData()
    {
        SaveSystem.Delete();
        Current = GameData.CreateDefault();
        Prefs.ResetToDefaults();
    }
}
