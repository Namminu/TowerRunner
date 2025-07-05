using System;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
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
		[Tooltip("Item Spawn Interval")]
		public float spawnInterval;
		[Tooltip("Drop Probability of Dead Monster Dropped")]
		public float dropChance;
	}

	[Tooltip("All Listed Items")]
	public List<ItemEntry> entries = new List<ItemEntry>();

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
}