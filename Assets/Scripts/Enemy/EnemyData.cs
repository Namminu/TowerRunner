using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
	[Serializable]
	public struct EnemyEntry
    {
		[Tooltip("Enemy Object Prefab")] 
		public AssetReferenceGameObject prefabRef;
		[Tooltip("Object Pool Size")] 
		public int poolSize;
		//[Tooltip("Spawn Interval")] 
		//public float spawnInterval;
		[Tooltip("Spawn Weight")]
		public float spawnWeight;
	}

	[Header("Prefab & Pooling")]
	[Tooltip("All Enemy Object Pooling Datas")]
	public List<EnemyEntry> entries = new List<EnemyEntry>();

	[Header("Spawn Settings")]
	public float minSpawnInterval = 0.5f;
	public float maxSpawnInterval = 2.5f;

	public AssetReferenceGameObject GetRandomSpawnTarget()
	{
		if (entries == null || entries.Count == 0)
		{
			Debug.LogWarning("Enemy entries가 비어있습니다.");
			return null;
		}

		float totalWeight = 0f;
		foreach (var entry in entries)
		{
			totalWeight += entry.spawnWeight;
		}
		if (totalWeight <= 0)
		{
			Debug.LogWarning("가중치의 합이 0 이하입니다. 리스트의 첫 번째 항목을 반환하거나 확인이 필요합니다.");
			return entries[0].prefabRef;
		}

		float randomValue = UnityEngine.Random.Range(0f, totalWeight);
		float currentWeightSum = 0f;

		foreach (var entry in entries)
		{
			currentWeightSum += entry.spawnWeight;

			if (randomValue <= currentWeightSum)
			{
				return entry.prefabRef;
			}
		}
		return entries[entries.Count - 1].prefabRef;
	}

	public float GetRandomSpawnInterval()
	{
		return UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
	}
}
