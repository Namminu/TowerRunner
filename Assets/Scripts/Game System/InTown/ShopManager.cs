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

	private bool _isInitialized = false;

	public void InitUI()
	{
		_isInitialized = false;
		slotPool.Clear();

		GameEvents.OnItemBuyConfirmed += OnBuyConfirmed;

		PopulateSlots();
		//if (!preloadHandle.IsValid())
		//{
		//	preloadHandle = itemSlotPrefab.LoadAssetAsync<GameObject>();
		//	preloadHandle.Completed += _ =>
		//	{
		//		if (this != null)
		//			PopulateSlots();
		//	};
		//}

		gameObject.SetActive(false);
	}

	//private void OnDisable()
	//{
	//	foreach (var s in slotPool)
	//		if (s)
	//			s.gameObject.SetActive(false);
	//}

	private void OnDisable()
	{
		GameEvents.OnItemBuyConfirmed -= OnBuyConfirmed;
	}

	private void OnDestroy()
	{
		slotPool.Clear();
		Dispose();
	}

	private void OnBuyConfirmed(ItemData item)
	{
		//// If Inven hasn't Enough Slot
		//if (!InvenManager.Instance.HasFreeSlot())
		//{
		//	GameEvents.RaiseInvenFull();
		//	return;
		//}

		//// If Player hans't Enough Gold
		//if (EconomyService.Gold < item.itemPrice)
		//{
		//	GameEvents.RaiseShortageGold();
		//	return;
		//}

		//// 리스트 내부 Missing 슬롯 제거
		//slotPool.RemoveAll(s => s == null);

		//EconomyService.TrySpendGold(item.itemPrice);
		///* 인벤에 아이템 추가 메서드 */
		//var slot = slotPool.FirstOrDefault(s => s.Data == item);
		//if (slot != null)
		//{
		//	slot.MarkPurchased();
		//	InvenManager.Instance.TryAddItem(item);
		//}
		//else Debug.LogError("Item Slot Missing Error");

		if (this == null) // 유니티 엔진에서 파괴된 상태라면
		{
			return; // 로직 실행 안 함
		}

		if (!_isInitialized)
		{
			Debug.LogWarning("상점 UI가 아직 완전히 로드되지 않았습니다.");
			return;
		}

		// 현재 이 메서드를 실행하는 ShopManager가 누구인지 식별하기 위해
		// 인스턴스의 고유 ID(GetHashCode)를 함께 찍습니다.
		Debug.Log($"[ShopManager-{this.GetHashCode()}] OnBuyConfirmed 시작: {item.itemName}");

		slotPool.RemoveAll(s => s == null);

		// null인 슬롯(이전 씬 잔해)은 절대 검색 대상에 포함시키지 않음
		var slot = slotPool.FirstOrDefault(s => s != null && s.Data.id == item.id);

		if (slot == null)
		{
			// 2. 왜 못 찾았는지 상세 로그 출력
			string poolInfo = string.Join(", ", slotPool.Select(s => s.Data?.itemName ?? "Null"));
			Debug.LogError($"[ShopManager-{this.GetHashCode()}] 슬롯 미매칭! 찾으려는것: {item.itemName}, 풀에 있는것들: [{poolInfo}]");
			return;
		}

		// 실제 구매 로직 실행
		if (EconomyService.Gold >= item.itemPrice && InvenManager.Instance.HasFreeSlot())
		{
			EconomyService.TrySpendGold(item.itemPrice);
			slot.MarkPurchased();
			InvenManager.Instance.TryAddItem(item);
		}
		else
		{
			GameEvents.RaiseInvenFull();
		}
	}

	private void PopulateSlots()
	{
		//if (slotPool.Count > 0)
		//{
		//	foreach(var slot in slotPool)
		//		slot.gameObject.SetActive(true);
		//	return;
		//}

		//if(!preloadHandle.IsValid())
		//{
		//	CreateByInstantiateAsync();
		//	return;
		//}

		//if(!preloadHandle.IsDone)
		//{
		//	preloadHandle.Completed += _ =>
		//	{
		//		if (this != null && isActiveAndEnabled)
		//			PopulateSlots();
		//		return;
		//	};
		//}

		//if(preloadHandle.Status == AsyncOperationStatus.Succeeded)
		//{
		//	var prefab = preloadHandle.Result;
		//	CreateSlotsFromPrefab(prefab);
		//}
		//else CreateByInstantiateAsync();

		// 로직 단순화: 이미 슬롯이 있다면 무시
		if (slotPool.Count > 0) return;

		// 어드레서블 로드 및 생성 로직
		var handle = itemSlotPrefab.LoadAssetAsync<GameObject>();
		handle.Completed += h =>
		{
			if (h.Status == AsyncOperationStatus.Succeeded)
			{
				CreateSlotsFromPrefab(h.Result);
				_isInitialized = true; // 생성이 완료된 시점에 플래그 true
			}
		};
	}

	private void CreateSlotsFromPrefab(GameObject prefab)
	{
		//Debug.Log("Is ShopManager CreateSlotsFromPrefab Called?");
		foreach (var data in shopItems)
		{
			var go = Instantiate(prefab, itemList);
			var slot = go.GetComponent<ShopItemSlot>();
			slot.Setup(data);
			slotPool.Add(slot);
		}
	}

	//private void CreateByInstantiateAsync()
	//{
	//	foreach (var data in shopItems)
	//	{
	//		var handle = itemSlotPrefab.InstantiateAsync(parent: itemList);
	//		handle.Completed += h =>
	//		{
	//			if(h.Status == AsyncOperationStatus.Succeeded)
	//			{
	//				var slot = h.Result.GetComponent<ShopItemSlot>();
	//				slot.Setup(data);
	//				slotPool.Add(slot);
	//			}
	//			else
	//			{
	//				Debug.LogError($"[ShopManager] InstantiateAsync Failed : {h.OperationException}");
	//			}
	//		};
	//	}
	//}

	public void Dispose()
	{
		if (preloadHandle.IsValid())
			Addressables.Release(preloadHandle);
	}
}
