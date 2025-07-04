using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
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

	public ItemEntry GetItemById(int id)
	{
		return entries.Find(item => item.data.id == id);
	}
}