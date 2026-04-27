using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class InvenUI : MonoBehaviour, ISceneUI
{
	[SerializeField] private AssetReference slotPrefab;
	[SerializeField] private Transform slotParent;

	private readonly List<InvenItemSlot> slotUIs = new();
	private AsyncOperationHandle<GameObject> preloadHandle;

	private void RefreshSlots()
	{
		var items = InvenManager.Instance.Items;
		bool isTower = SceneConfig.CurrentScene == Scenes.Tower;

		for (int i = 0; i < slotUIs.Count; i++)
		{
			slotUIs[i].SetData(i < items.Count ? items[i] : null);
			slotUIs[i].Button.interactable = isTower && (i < items.Count);
		}
	}

	private void UseItemAt(int idx)
	{
		var items = InvenManager.Instance.Items;
		if(idx < items.Count)
		{
			var item = items[idx];
			InvenManager.Instance.RemoveItem(item);
			item.Apply(Player.Instance);
		}
	}

	private void OnDestroy()
	{
		if(preloadHandle.IsValid())
			Addressables.Release(preloadHandle);
		GameEvents.OnInventoryChanged -= RefreshSlots;
	}

	public void InitUI()
	{
		preloadHandle = slotPrefab.LoadAssetAsync<GameObject>();
		preloadHandle.Completed += handle =>
		{
			if(handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError("Inven Slot Prefab Load Failed");
				return;
			}

			for(int i = 0; i < InvenManager.Instance.MaxInvenSlots; i++)
			{
				var go = Instantiate(handle.Result, slotParent);
				var slot = go.GetComponent<InvenItemSlot>();
				slot.SetEmpty();
				slotUIs.Add(slot);

				int index = i;
				slot.Button.onClick.AddListener(() => UseItemAt(index));
			}
			RefreshSlots();
		};
		GameEvents.OnInventoryChanged += RefreshSlots;
	}
}
