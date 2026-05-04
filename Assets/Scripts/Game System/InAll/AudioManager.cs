using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoBehaviour, IInitializable
{
    public static AudioManager Instance { get; private set; }

	[Header("Volume Settings")]
	[Range(0f, 1f)]
	private float _masterVolume = 0.5f;
	public float MasterVolume => _masterVolume;

	[Header("Audio Sources")]
	[SerializeField] public AssetReferenceT<AudioConfig> audioDataRef;
	private AudioConfig _audioConfig;

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
		StartCoroutine(InitRoutine());

		SetMasterVolume(Prefs.MasterVolume);
		ApplyVolume();
	}

	private IEnumerator InitRoutine()
	{
		var handle = audioDataRef.InstantiateAsync();
		yield return handle;

		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"{audioDataRef.RuntimeKey} Load Failed");
			yield break;
		}

		_audioConfig = handle.Result.GetComponent<AudioConfig>();
		if(_audioConfig == null)
		{
			Debug.LogError($"{audioDataRef.RuntimeKey} Load Failed");
			yield break;
		}
		_audioConfig.Initialize();
	}

	public void SetMasterVolume(float value)
	{
		_masterVolume = Mathf.Clamp01(value);
		ApplyVolume();
	}

	public async void PlayBGM(AudioID id)
	{
		if (_audioConfig == null)
		{
			Debug.LogError("AudioConfig not loaded yet.");
			return;
		}
		var clipRef = _audioConfig.GetAudioClip(id);
		if (clipRef == null)
		{
			Debug.LogError($"AudioClip for {id} not found.");
			return;
		}
		var handle = clipRef.LoadAssetAsync();
		await handle.Task;
		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"Failed to load AudioClip for {id}.");
			return;
		}
		var clip = handle.Result;
		var audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = _audioConfig.entries[id].volume * MasterVolume;
		audioSource.loop = true;
		audioSource.Play();
	}

	public async void PlaySound(AudioID id)
	{
		if (_audioConfig == null)
		{
			Debug.LogError("AudioConfig not loaded yet.");
			return;
		}
		var clipRef = _audioConfig.GetAudioClip(id);
		if (clipRef == null)
		{
			Debug.LogError($"AudioClip for {id} not found.");
			return;
		}
		var handle = clipRef.LoadAssetAsync();
		await handle.Task;
		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError($"Failed to load AudioClip for {id}.");
			return;
		}
		var clip = handle.Result;
		var audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = _audioConfig.entries[id].volume * MasterVolume;
		audioSource.Play();
	}
}
