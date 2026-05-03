using UnityEngine;
using UnityEngine.AddressableAssets;

public class AudioManager : MonoBehaviour, IInitializable
{
    public static AudioManager Instance { get; private set; }

	[Header("Volume Settings")]
	[Range(0f, 1f)]
	private float _masterVolume = 0.5f;
	public float MasterVolume => _masterVolume;

	[Header("Audio Sources")]
	[SerializeField] public AssetReferenceT<AudioConfig> audioDataRef;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		ApplyVolume();
	}

	private void ApplyVolume()
	{
		AudioListener.volume = _masterVolume;
		Prefs.MasterVolume = _masterVolume;
	}

	public void Init()
    {
		SetMasterVolume(Prefs.MasterVolume);
		ApplyVolume();
	}

	public void SetMasterVolume(float value)
	{
		_masterVolume = Mathf.Clamp01(value);
		ApplyVolume();
	}

}
