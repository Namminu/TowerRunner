using UnityEngine;

public static class PrefKeys
{
    public const string MasterVolume = "MasterVolume";
    public const string DisplayBrightness = "DisplayBrightness";

}

public static class Prefs
{
    private const float DefaultVolume = 0.5f;
    private const float DefaultBrightness = 0.5f;

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(PrefKeys.MasterVolume, DefaultVolume);
        set => PlayerPrefs.SetFloat(PrefKeys.MasterVolume, Mathf.Clamp01(value));
    }

    public static float DisplayBrightness
    {
		get => PlayerPrefs.GetFloat(PrefKeys.DisplayBrightness, DefaultBrightness);
		set => PlayerPrefs.SetFloat(PrefKeys.DisplayBrightness, Mathf.Clamp01(value));
	}

    public static void Save() => PlayerPrefs.Save();

    public static void ResetToDefaults()
    {
        MasterVolume = DefaultVolume;
        DisplayBrightness = DefaultBrightness;
        
        Save();
    }
}
