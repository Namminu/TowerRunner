using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
	[Tooltip("All Listed Items")]
	public List<ItemData> allItems = new List<ItemData>();

	public ItemData GetItemById(int id)
	{
		return allItems.Find(item => item.id == id);
	}
}