using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TownSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Scene&Setting")]
	[SerializeField] private Scenes nextSceneName;
	[SerializeField] private SceneConfig sceneConfig;
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

	private void Awake()
	{
		InitUI();
	}

	public void InitUI()
	{
		gameStartBtn.onClick.RemoveAllListeners();
		settingBtn.onClick.RemoveAllListeners();
		shopNpcBtn.onClick.RemoveAllListeners();
		enforceNpcBtn.onClick.RemoveAllListeners();

		gameStartBtn.onClick.AddListener(() => StartCoroutine(GameStartBtn()));
		settingBtn.onClick.AddListener(() => SettingUIToggle());
		shopNpcBtn.onClick.AddListener(() => ShopUIToggle());
		enforceNpcBtn.onClick.AddListener(() => EnforceUIToggle());
	}

	private IEnumerator GameStartBtn()
	{
		yield return ManagersInitializer.Instance.InitializeSceneManagers(nextSceneName);
		yield return sceneConfig.LoadSceneRoutine(nextSceneName);
	}

	private void ShopUIToggle()
	{
		if(enforceScroll.IsActive())
		{
			enforceScroll.gameObject.SetActive(false);
			return;
		}

		gameStartBtn.gameObject.SetActive(false);
		shopScroll.gameObject.SetActive(true);
	}

	private void EnforceUIToggle()
	{
		if (shopScroll.IsActive())
		{
			shopScroll.gameObject.SetActive(false);
			return;
		}

		gameStartBtn.gameObject.SetActive(false);
		enforceScroll.gameObject.SetActive(true);
	}

	private void SettingUIToggle() 
		=> settingPanel.gameObject.SetActive(true);

	/// <summary>
	/// Send Player's Left Gold Amount to Parameter
	/// </summary>
	public void UpdateGoldAmount(int amount)
	{
		string left = amount.ToString();
		goldAmount.text = left;
	}
}
 