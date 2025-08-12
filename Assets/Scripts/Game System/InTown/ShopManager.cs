using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopManager : MonoBehaviour, ISceneUI
{
	[SerializeField] private AssetReferenceGameObject itemSlotPrefab;
	[SerializeField] private Transform itemList;
	[SerializeField] private List<ItemData> shopItems;

	private AsyncOperationHandle<GameObject> preloadHandle;
	private List<ShopItemSlot> slotPool = new();

	public void InitUI()
	{
		if(!preloadHandle.IsValid())
		{
			preloadHandle = itemSlotPrefab.LoadAssetAsync<GameObject>();
			preloadHandle.Completed += _ =>
			{
				if (this != null && isActiveAndEnabled)
					PopulateSlots();
			};
		}
	}

	private void OnEnable()
	{
		GameEvents.OnItemBuyConfirmed += OnBuyConfirmed;
		PopulateSlots();
	}

	private void OnDisable()
	{
		GameEvents.OnItemBuyConfirmed -= OnBuyConfirmed;
		foreach (var s in slotPool)
			if(s)
				s.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void OnBuyConfirmed(ItemData item)
	{
		// If Inven hasn't Enough Slot
		if (!InvenManager.Instance.HasFreeSlot())
		{
			GameEvents.RaiseInvenFull();
			return;
		}

		// If Player hans't Enough Gold
		if (EconomyService.Gold < item.itemPrice)
		{
			GameEvents.RaiseShortageGold();
			return;
		}
		 
		EconomyService.TrySpendGold(item.itemPrice);
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

		if(!preloadHandle.IsValid())
		{
			CreateByInstantiateAsync();
			return;
		}

		if(!preloadHandle.IsDone)
		{
			preloadHandle.Completed += _ =>
			{
				if (this != null && isActiveAndEnabled)
					PopulateSlots();
				return;
			};
		}

		if(preloadHandle.Status == AsyncOperationStatus.Succeeded)
		{
			var prefab = preloadHandle.Result;
			CreateSlotsFromPrefab(prefab);
		}
		else CreateByInstantiateAsync();
	}

	private void CreateSlotsFromPrefab(GameObject prefab)
	{
		foreach (var data in shopItems)
		{
			var go = Instantiate(prefab, itemList);
			var slot = go.GetComponent<ShopItemSlot>();
			slot.Setup(data);
			slotPool.Add(slot);
		}
	}

	private void CreateByInstantiateAsync()
	{
		foreach (var data in shopItems)
		{
			var handle = itemSlotPrefab.InstantiateAsync(parent: itemList);
			handle.Completed += h =>
			{
				if(h.Status == AsyncOperationStatus.Succeeded)
				{
					var slot = h.Result.GetComponent<ShopItemSlot>();
					slot.Setup(data);
					slotPool.Add(slot);
				}
				else
				{
					Debug.LogError($"[ShopManager] InstantiateAsync Failed : {h.OperationException}");
				}
			};
		}
	}

	public void Dispose()
	{
		if (preloadHandle.IsValid())
			Addressables.Release(preloadHandle);
	}
}
