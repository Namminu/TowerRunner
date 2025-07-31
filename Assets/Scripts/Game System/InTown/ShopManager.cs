using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopManager : MonoBehaviour
{
	[SerializeField] private AssetReference itemSlotPrefab;
	[SerializeField] private Transform itemList;
	[SerializeField] private List<ItemData> shopItems;

	private AsyncOperationHandle<GameObject> preloadHandle;
	private List<ShopItemSlot> slotPool = new();

	public void Init()
	{
		preloadHandle = itemSlotPrefab.LoadAssetAsync<GameObject>();
	}

	private void OnEnable()
	{
		GameEvents.OnItemBuyConfirmed += OnBuyConfirmed;
		PopulateSlots();
	}

	private void OnDisable()
	{
		GameEvents.OnItemBuyConfirmed -= OnBuyConfirmed;
		Dispose();
	}

	private void OnBuyConfirmed(ItemData item)
	{
		// If Inven hasn't Enough Slot
		//if (InvenManager.HasFreeSlot())
		//{
		//	GameEvents.RaiseInvenFull();
		//	return;
		//}

		// If Player hans't Enough Gold
		if (Player.Instance.PlayerGold < item.itemPrice)
		{
			GameEvents.RaiseShortageGold();
			return;
		}

		Player.Instance.PlayerGold -= item.itemPrice;
		/* 인벤에 아이템 추가 메서드 */
		var slot = slotPool.First(s => s.Data == item);
		slot.MarkPurchased();
	}

	private void PopulateSlots()
	{
		if(slotPool.Count > 0)
		{
			foreach(var slot in slotPool)
				slot.gameObject.SetActive(true);
			return;
		}

		if(preloadHandle.Status == AsyncOperationStatus.Succeeded)
		{
			var prefab = preloadHandle.Result;
			foreach(var data in shopItems)
			{
				var go = Instantiate(prefab, itemList);
				var slot = go.GetComponent<ShopItemSlot>();
				slot.Setup(data);
				slotPool.Add(slot);
			}
		}
		else
		{
			foreach(var data in shopItems)
			{
				var handle = itemSlotPrefab.InstantiateAsync(parent: itemList);
				handle.Completed += h =>
				{
					var slot = h.Result.GetComponent<ShopItemSlot>();
					slot.Setup(data);
					slotPool.Add(slot);
				};
			}
		}
	}

	public void Dispose()
	{
		if (preloadHandle.IsValid())
			Addressables.Release(preloadHandle);
	}
}
