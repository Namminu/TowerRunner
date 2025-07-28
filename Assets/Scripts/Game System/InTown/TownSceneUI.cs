using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TownSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Setting")]
	[SerializeField] private Button settingBtn;
	[SerializeField] private Image settingPopUI;

	[Header("Shop NPC")]
	[SerializeField] private Button shopBtn;
	[SerializeField] private Image shopScroll;
	[SerializeField] private Button[] itemBtns; 

	[Header("Enforce NPC")]
	[SerializeField] private Button enforceBtn;
	[SerializeField] private Image enforceScroll;

	[Header("GameStart")]
	[SerializeField] private Button gameStartBtn;

	[Header("Bottom Item List")]
	[SerializeField] private Image ItemNo1;
	[SerializeField] private Image ItemNo2;
	[SerializeField] private Image ItemNo3;
	private List<Image> ItemList = new();

	[Header("PopUp UI")]
	[SerializeField] private Image PopupUI;

	public void InitUI()
	{
		settingBtn.onClick.AddListener(() => SettingToggle());
		shopBtn.onClick.AddListener(() => ShopToggle());
		enforceBtn.onClick.AddListener(() => EnforceToggle());
	}

	#region --- Shop Initialize ---
	private void ShopToggle()
	{
		if (shopScroll.IsActive())
		{
			shopScroll.gameObject.SetActive(false);
			gameStartBtn.gameObject.SetActive(true);
			return;
		}

		if (gameStartBtn.IsActive())
			gameStartBtn.gameObject.SetActive(false);

		shopScroll.gameObject.SetActive(true);
	}

	private void ItemBtnClick()
	{

	}
	#endregion

	#region --- Enforce Initialize 
	private void EnforceToggle()
	{
		if (enforceScroll.IsActive())
		{
			enforceScroll.gameObject.SetActive(false);
			gameStartBtn.gameObject.SetActive(true);
			return;
		}

		if (gameStartBtn.IsActive())
			gameStartBtn.gameObject.SetActive(false);

		enforceScroll.gameObject.SetActive(true);
	}
	#endregion

	#region --- Setting Initialize ---
	private void SettingToggle()
	{
		if (settingPopUI.IsActive())
		{
			settingPopUI.gameObject.SetActive(false);
			return;
		}

		settingPopUI.gameObject.SetActive(true);
	}
	#endregion
}
 