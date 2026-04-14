using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TowerSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Top Zone")]
	[SerializeField] private TowerTopUI TopUI;

	[Header("Player Health UI")]
	[SerializeField] private Image PlayerHealthBar;

	[Header("Panel Zone")]
	[SerializeField] private TowerPanelUI PanelUI;

	public void InitUI()
	{
		Debug.Log("Tower Scene UI Init Called");

		if(TopUI == null)
		{
			TopUI = GetComponentInChildren<TowerTopUI>();
			if (TopUI)
			{
				TopUI.InitUI();
			}
		}

		if(PanelUI == null)
		{
			PanelUI = GetComponentInChildren<TowerPanelUI>();
			if (PanelUI)
			{
				PanelUI.InitUI();
			}
		}

		Player.Instance.OnHealthChanged += HandlePlayerHealthOnUI;
	}

	private void HandlePlayerHealthOnUI(float healthRatio)
	{
		if (PlayerHealthBar == null)
		{
			Debug.LogWarning("Player Health Bar is not assigned in TowerSceneUI.");
			return;
		}
		PlayerHealthBar.fillAmount = healthRatio;
	}

	private void OnDisable()
	{
		Player.Instance.OnHealthChanged -= HandlePlayerHealthOnUI;
	}
}
