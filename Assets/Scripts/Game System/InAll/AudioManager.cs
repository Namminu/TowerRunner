using UnityEngine;

public class AudioManager : MonoBehaviour, IInitializable
{
    public static AudioManager Instance { get; private set; }

	[Range(0f, 1f)]
	private float _masterVolume = 0.5f;
	public float MasterVolume => _masterVolume;

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
	}

	public void Init()
    {
		ApplyVolume();
	}

	public void SetMasterVolume(float value)
	{
		_masterVolume = Mathf.Clamp01(value);
		ApplyVolume();
	}

}
