
using System.Collections.Generic;
using UnityEngine;

public class InvenManager : MonoBehaviour, IInitializable
{
	public static InvenManager Instance { get; private set; }

	[Header("Slot Prefab Setting")]
	[SerializeField] private int maxSlots = 3;
	public int MaxInvenSlots => maxSlots;

	private readonly List<ItemData> items = new();
	public IReadOnlyList<ItemData> Items => items;

	private void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void TryAddItem(ItemData item)
	{
		if (!HasFreeSlot())
		{
			GameEvents.RaiseInvenFull();
			return;
		}
		items.Add(item);
		GameEvents.RaiseInventoryChanged();
		FirebaseManager.LogEvent($"{item.name} Add to Inven");
	}

	public void RemoveItem(ItemData item)
	{
		if (items.Remove(item))
		{
			GameEvents.RaiseInventoryChanged();
			FirebaseManager.LogEvent($"{item.name} Spend from Inven");
		}
	}

	public bool HasFreeSlot() => items.Count < maxSlots;

	public void Init()
	{

	}

	public void ReplaceAllFronmSave(IEnumerable<ItemData> newItems)
	{
		items.Clear();
		foreach(var it in newItems)
		{
			if (it == null) continue;
			if (items.Count >= maxSlots) break;
			items.Add(it);
		}
		GameEvents.RaiseInventoryChanged();
	}
}
