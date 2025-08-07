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
        Current = await SaveSystem.LoadAsync();

        AudioManager.Instance.SetMasterVolume(Prefs.MasterVolume);
        BrightnessManager.Instance.SetBrightness(Prefs.DisplayBrightness);
    }

	/// <summary>
	/// Save all game data recorded to date
	/// </summary>
	public static async Task SaveAllAsync()
    {
        Prefs.Save();

        await SaveSystem.SaveAsync(Current);
    }

	/// <summary>
	/// Delete game progress data
	/// </summary>
	public static void ResetAll()
    {
        SaveSystem.Delete();
        Current = GameData.CreateDefault();
        Prefs.ResetToDefaults();
    }
}
