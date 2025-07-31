using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

	private void OnDestroy()
	{
		GameEvents.OnItemBuyConfirmed -= TryAddItem;
	}

	private void TryAddItem(ItemData item)
	{
		if(!HasFreeSlot())
		{
			GameEvents.RaiseInvenFull();
			return;
		}
		items.Add(item);
		GameEvents.RaiseInventoryChanged();
	}

	public void RemoveItem(ItemData item)
	{
		if (items.Remove(item))
			GameEvents.RaiseInventoryChanged();
	}

	public void Init()
	{
		GameEvents.OnItemBuyConfirmed += TryAddItem;
	}

	public bool HasFreeSlot() => items.Count < maxSlots;
}
