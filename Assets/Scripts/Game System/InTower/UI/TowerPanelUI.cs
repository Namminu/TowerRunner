using UnityEngine;
using UnityEngine.UI;

public class TowerPanelUI : MonoBehaviour
{
	[SerializeField] private Image settingPanel;
	[SerializeField] private Image gameOverPanel;

	public void PopupSettingPanel()
	{
		settingPanel.gameObject.SetActive(true);
	}

	public void PopupGameOverPanel()
	{
		gameOverPanel.gameObject.SetActive(true);
	}
}
