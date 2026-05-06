using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
	public static ItemDatabase Instance { get; private set; }

	[Serializable]
	public struct ItemEntry
	{
		[Tooltip("Item Data")]
		public ItemData data;
		[Tooltip("Item Pooling Initial Size")]
		public int poolSize;
		//[Tooltip("Item Spawn Interval")]
		//public float spawnInterval;
		[Tooltip("Drop Probability of Dead Monster Dropped")]
		public float dropChance;
	}

	[Tooltip("All Listed Items")]
	public List<ItemEntry> entries = new List<ItemEntry>();

	[Header("Spawn Settings")]
	public float minSpawnInterval = 0.5f;
	public float maxSpawnInterval = 2.5f;

	private void OnEnable()
	{
		Instance = this;
	}

	public ItemEntry GetItemById(int id)
	{
		return entries.Find(item => item.data.id == id);
	}

	public static ItemData GetRandomDrop()
	{
		if(UnityEngine.Random.value >= 0.5f) return null;

		float total = 0f;
		foreach (var e in Instance.entries)
		{
			total += e.dropChance;
		}

		if (total != 100f || total <= 0f)
		{
			Debug.LogError("ItemDatabase : DropChance Error");
			return null;
		}

		float roll = UnityEngine.Random.value * total;
		foreach (var e in Instance.entries)
		{
			if (roll < e.dropChance)
				return e.data;
			roll -= e.dropChance;
		}
		return Instance.entries[Instance.entries.Count - 1].data;
	}

	public float GetRandomSpawnInterval()
	{
		return UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
	}

	public ItemData GetRandowmSpawnTarget()
	{
		if (entries == null || entries.Count == 0)
		{
			Debug.LogWarning("Item entries가 비어있습니다.");
			return null;
		}

		float totalWeight = 0f;
		foreach (var entry in entries)
		{
			totalWeight += entry.dropChance;
		}
		if (totalWeight <= 0)
		{
			Debug.LogWarning("가중치의 합이 0 이하입니다. 리스트의 첫 번째 항목을 반환하거나 확인이 필요합니다.");
			return entries[0].data;
		}

		float randomValue = UnityEngine.Random.Range(0f, totalWeight);
		float currentWeightSum = 0f;

		foreach (var entry in entries)
		{
			currentWeightSum += entry.dropChance;

			if (randomValue <= currentWeightSum)
			{
				return entry.data;
			}
		}
		return entries[entries.Count - 1].data;
	}
}