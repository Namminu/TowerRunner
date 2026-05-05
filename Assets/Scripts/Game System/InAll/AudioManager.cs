using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoBehaviour, IInitializable
{
    public static AudioManager Instance { get; private set; }

	[Header("Volume Settings")]
	[UnityEngine.Range(0f, 1f)]
	private float _masterVolume = 0.5f;
	public float MasterVolume => _masterVolume;

	[Header("Audio Sources")]
	[SerializeField] public AssetReferenceT<AudioConfig> audioDataRef;
	private AudioConfig _audioConfig;

	private AudioSource _bgmSource;
	private List<AudioSource> _sfxSource = new();

	private const int SFX_POOL_COUNT = 10;

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
		_bgmSource = gameObject.AddComponent<AudioSource>();
		_bgmSource.loop = true;

		for(int i = 0; i< SFX_POOL_COUNT; i++)
		{
			var source = gameObject.AddComponent<AudioSource>();
			_sfxSource.Add(source);
		}

		SetMasterVolume(Prefs.MasterVolume);
		ApplyVolume();

		StartCoroutine(InitRoutine());
	}

	private IEnumerator InitRoutine()
	{
		var handle = audioDataRef.LoadAssetAsync<AudioConfig>();
		yield return handle;
		if (handle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Failed to load AudioConfig.");
			yield break;
		}
		_audioConfig = handle.Result;
		yield return _audioConfig.Initialize();
	}

	public void SetMasterVolume(float value)
	{
		_masterVolume = Mathf.Clamp01(value);
		ApplyVolume();
	}

	public void PlayBGM(AudioID id)
	{
		if (!_audioConfig.loadedClips.TryGetValue(id, out var audioStruct))
		{
			Debug.LogAssertion($"Audio clip for ID {id} not found.");
			return;
		}

		if (_bgmSource.clip == audioStruct.clip && _bgmSource.isPlaying)
		{
			return;
		}

		_bgmSource.Stop(); // 이전 BGM 중단
		_bgmSource.clip = audioStruct.clip;
		_bgmSource.volume = audioStruct.volume * MasterVolume;
		_bgmSource.Play();
	}

	public void PauseBGM()
	{
		if(_bgmSource == null)
		{
			Debug.Log("BGM Audio Source Null Error");
			return;
		}
		_bgmSource.Pause();
	}

	public void ResumeBGM()
	{
		if (_bgmSource == null)
		{
			Debug.Log("BGM Audio Source Null Error");
			return;
		}
		_bgmSource.UnPause();
	}

	public void PlaySound(AudioID id)
	{
		if (!_audioConfig.loadedClips.TryGetValue(id, out var audioStruct))
		{
			Debug.LogAssertion($"Audio clip for ID {id} not found.");
			return;
		}

		var source = _sfxSource.Find(s => !s.isPlaying);
		if (source == null) source = _sfxSource[0];

		float finalVolume = audioStruct.volume * MasterVolume;
		source.PlayOneShot(audioStruct.clip, finalVolume);

		//AudioStruct audioStruct = _audioConfig.GetAudioClip(id);
		//if(audioStruct.clip == null)
		//{
		//	Debug.LogAssertion($"Audio clip for ID {id} is null.");
		//	return;
		//}

		//source.clip = audioStruct.clip;
		//source.volume = _audioConfig.loadedClips[id].volume * MasterVolume;
		//source.PlayOneShot(source.clip);
	}
}
