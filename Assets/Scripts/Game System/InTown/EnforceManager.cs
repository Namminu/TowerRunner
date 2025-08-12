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
				ShowPage(idx);
			});
		}

		enforceBtn.onClick.AddListener(OnEnforceClicked);

		warningUI.SetActive(false);

		ShowPage(selectedIndex);
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
		costText.text = costInt.ToString();

		bool canUpgrade = level < data.maxLevel && EconomyService.Gold >= costInt;
		enforceBtn.interactable = canUpgrade;
	}

	private void OnEnforceClicked()
	{
		int level = UpgradeService.GetLevel(selectedIndex);

		int cost = EnforceService.GetCost(selectedIndex, level);

		if(!EconomyService.TrySpendGold(cost))
		{
			warningUI.SetActive(true);
			return;
		}

		int newLevel = level + 1;
		UpgradeService.SetLevel(selectedIndex, newLevel);

		float newValue = EnforceService.GetValue(selectedIndex, newLevel);
		Player.Instance?.ApplyUpgrade((EnforceType)selectedIndex, newValue);

		ShowPage(selectedIndex);	
	}

	private void OnEnable()
	{
		GameEvents.OnGoldChanged += _ => ShowPage(selectedIndex);
	}
	private void OnDisable()
	{
		GameEvents.OnGoldChanged -= _ => ShowPage(selectedIndex);
	}
}
