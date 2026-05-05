using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnforceManager : MonoBehaviour, ISceneUI
{
	[Header("DataBase")]
	[SerializeField] private EnforceDatabase enforceDB;

	[Header("Button Group")]
	[SerializeField] private Button[] attributeBtns;

	[Header("Page UI")]
	[SerializeField] private Text titleText;
	[SerializeField] private Text descText;
	[SerializeField] private Text enforceLevel;
	[SerializeField] private Button enforceBtn;
	[SerializeField] private Text costText;

	[Header("Warning UI")]
	[SerializeField] private GameObject warningUI;
	[SerializeField] private Text warningText;

	private int selectedIndex;

	public void InitUI()
	{
		selectedIndex = 0;
		for(int i = 0; i< attributeBtns.Length; i++)
		{
			int idx = i;
			var label = attributeBtns[i].GetComponentInChildren<Text>();
			label.text = enforceDB.enforceDB[idx].displayName;

			attributeBtns[i].onClick.AddListener(() =>
			{
				AudioManager.Instance.PlaySound(AudioID.ButtonClick);
				ShowPage(idx);
			});
		}

		enforceBtn.onClick.AddListener(OnEnforceClicked);
		warningUI.SetActive(false);
		ShowPage(selectedIndex);

		GameEvents.OnGoldChanged += HandleGoldChange;
	}

	private void ShowPage(int idx)
	{
		selectedIndex = idx;
		var data = EnforceService.GetData(idx);

		titleText.text = data.displayName;
		descText.text = data.description;

		int level = UpgradeService.GetLevel(idx);
		enforceLevel.text = level.ToString();

		int costInt = EnforceService.GetCost(idx, level);
		if(level >= enforceDB.enforceDB[idx].maxLevel)
		{
			costText.text = "MAX";
		}
		else costText.text = costInt.ToString();

		bool canUpgrade = level < data.maxLevel /*&& EconomyService.Gold >= costInt*/;
		enforceBtn.interactable = canUpgrade;
	}

	private void OnEnforceClicked()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);

		int level = UpgradeService.GetLevel(selectedIndex);

		int cost = EnforceService.GetCost(selectedIndex, level);

		if(!EconomyService.TrySpendGold(cost))
		{
			warningText.text = "보유 골드가 충분하지 않습니다";
			warningUI.SetActive(true);
			return;
		}

		int newLevel = level + 1;
		UpgradeService.SetLevel(selectedIndex, newLevel);

		float newValue = EnforceService.GetValue(selectedIndex, newLevel);
		Player.Instance.ApplyUpgrade((EnforceType)selectedIndex, newValue);

		ShowPage(selectedIndex);	
	}

	private void OnDestroy()
	{
		GameEvents.OnGoldChanged -= HandleGoldChange;

		enforceBtn.onClick.RemoveAllListeners();
		for (int i = 0; i < attributeBtns.Length; i++)
		{
			attributeBtns[i].onClick.RemoveAllListeners();
		}
	}

	private void HandleGoldChange(int gold)
	{
		if (this == null) return;
		ShowPage(selectedIndex);
	}
}
