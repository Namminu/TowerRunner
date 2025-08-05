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
		var data = enforceDB.enforceDB[idx];

		titleText.text = data.displayName;
		descText.text = data.description;

		int level = Player.Instance.GetEnforceLevel(idx);
		enforceLevel.text = level.ToString();

		float cost = data.initCost * Mathf.Pow(data.costMultiplier, level - data.initLevel);
		int costInt = Mathf.CeilToInt(cost);
		costText.text = costInt.ToString();

		bool canUpgrade = level < data.maxLevel
						&& Player.Instance.PlayerGold >= costInt;
		enforceBtn.interactable = canUpgrade;
	}

	private void OnEnforceClicked()
	{
		var data = enforceDB.enforceDB[selectedIndex];
		int level = Player.Instance.GetEnforceLevel(selectedIndex);

		float costF = data.initCost * Mathf.Pow(data.costMultiplier, level - data.initLevel);
		int cost = Mathf.CeilToInt(costF);

		if(!Player.Instance.SpendGold(cost))
		{
			warningUI.SetActive(true);
			return;
		}

		int newLevel = level + 1;
		Player.Instance.SetEnforceLevel(selectedIndex, newLevel);

		float newValue = data.initValue + data.valueIncrement * (newLevel - data.initLevel);
		ApplyUpgrade(selectedIndex, newValue);

		ShowPage(selectedIndex);
		
	}

	private void ApplyUpgrade(int idx, float value)
	{
		switch(idx)
		{
			case 0:
				Player.Instance.SetMaxHealth(value);
				break;

			case 1:
				Player.Instance.SetFatal(value);
				break;

			case 2:
				Player.Instance.SetAttackPower(value);
				break;

			case 3:
				Player.Instance.SetAttackRange(value);
				break;

			default: break;
		}
	}
}
