using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum  AudioID
{
    MainBGM,
	TownBGM,
    TowerBGM,
    ButtonClick,
    EnemyHit,
    PlayerHit,
    HealthPotion,
    BattleBooster,
    Gold,
    FatalExlixir,
    Magnet,
    Shield,
    PlayerDeath,
    PlayerAttack
}

public struct AudioStruct
{
    public AudioClip clip;
    public float volume;
}

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Scriptable Objects/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    [Serializable]
    public struct AudioEntry
    {
        public AudioID id;
        public AssetReferenceT<AudioClip> clipRef;
        [Range(0f, 1f)] public float volume;
	}
	[SerializeField] private List<AudioEntry> audioList = new();

	public Dictionary<AudioID, AudioStruct> loadedClips = new();

    public async Task Initialize()
    {
		List<Task> loadTasks = new();
		foreach (var entry in audioList)
		{
			loadTasks.Add(LoadAndCached(entry));
		}
		await Task.WhenAll(loadTasks);
	}

    private async Task LoadAndCached(AudioEntry entry)
    {
        var handle = entry.clipRef.LoadAssetAsync();
        await handle.Task;
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			if(!loadedClips.ContainsKey(entry.id))
            {
				AudioStruct newAudio = new AudioStruct
                {
                    clip = handle.Result,
                    volume = entry.volume
                };
				loadedClips.Add(entry.id, newAudio);
			}
		}
        else
        {
            Debug.LogError($"Failed to load audio clip for AudioID {entry.id}");
		}
	}
}
