using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum UISwitcher
{
	GameStart,
	Shop,
	Enforce
}

public class TownSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Scene&Setting")]
	[SerializeField] private Scenes nextSceneName;
	[SerializeField] private SetupManager setupManager;

	[Header("Top Line")]
	[SerializeField] private Button gameStartBtn;

	[SerializeField] private Button settingBtn;
	[SerializeField] private Image settingPanel;

	[SerializeField] private Text goldAmount;
	[SerializeField] private Text highScore;

	[Header("Shopping")]
	[SerializeField] private Button shopNpcBtn;
	[SerializeField] private Image shopScroll;
	[SerializeField] private ShopManager shopManager;

	[Header("Enforcing")]
	[SerializeField] private Button enforceNpcBtn;
	[SerializeField] private Image enforceScroll;
	[SerializeField] private EnforceManager enforceManager;

	public void InitUI()
	{
		gameStartBtn.onClick.RemoveAllListeners();
		settingBtn.onClick.RemoveAllListeners();
		shopNpcBtn.onClick.RemoveAllListeners();
		enforceNpcBtn.onClick.RemoveAllListeners();

		gameStartBtn.onClick.AddListener(() => GameStartBtn());
		settingBtn.onClick.AddListener(() => SettingUIToggle());
		shopNpcBtn.onClick.AddListener(() => ShopUIToggle());
		enforceNpcBtn.onClick.AddListener(() => EnforceUIToggle());

		SwitchingUI(UISwitcher.GameStart);
	}

	private void GameStartBtn()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		ManagersInitializer.Instance.SceneLoad(nextSceneName);
		FirebaseManager.LogEvent("Game Start : Tower");
	}

	private void ShopUIToggle()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		if (shopScroll.IsActive())
		{
			SwitchingUI(UISwitcher.GameStart);
		}
		else
		{
			SwitchingUI(UISwitcher.Shop);
		}
	}

	private void EnforceUIToggle()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		if (enforceScroll.IsActive())
		{
			SwitchingUI(UISwitcher.GameStart);
		}
		else
		{
			SwitchingUI(UISwitcher.Enforce);
		}
	}

	private void SettingUIToggle()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);
		settingPanel.gameObject.SetActive(true);
	}

	/// <summary>
	/// Send Player's Left Gold Amount to Parameter
	/// </summary>
	public void UpdateGoldAmount(int amount)
	{
		string left = amount.ToString();
		goldAmount.text = left;
	}

	private void SwitchingUI(UISwitcher UIType)
	{
		switch(UIType)
		{
			case UISwitcher.GameStart:
				gameStartBtn.gameObject.SetActive(true);
				shopScroll.gameObject.SetActive(false);
				enforceScroll.gameObject.SetActive(false);
				break;
			case UISwitcher.Enforce:
				gameStartBtn.gameObject.SetActive(false);
				shopScroll.gameObject.SetActive(false);
				enforceScroll.gameObject.SetActive(true);
				break;
			case UISwitcher.Shop:
				gameStartBtn.gameObject.SetActive(false);
				shopScroll.gameObject.SetActive(true);
				enforceScroll.gameObject.SetActive(false);
				break;
		}	
	}	
}
