using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Scriptable Objects/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    [Serializable]
    public struct AudioEntry
    {
        public AssetReferenceT<AudioClip> clipRef;
        [Range(0f, 1f)] public float volume;
	}

    public Dictionary<AudioID, AudioEntry> entries = new();

    public void Initialize()
    {

    }

    public AssetReferenceT<AudioClip> GetAudioClip(AudioID audioID)
    {
        if (entries.TryGetValue(audioID, out var entry))
        {
            return entry.clipRef;
        }
        else
        {
            Debug.LogError($"[AudioConfig] AudioID {audioID} not found in entries.");
            return null;
		}
	}
}
