using UnityEngine;
using UnityEngine.UI;

public class TowerPanelUI : MonoBehaviour
{
	[Header("Setting")]
	[SerializeField] private Image SettingPanel;
	[SerializeField] private Button SettingCloseButton;

	[Header("Game Over")]
	[SerializeField] private Image GameOverPanel;

	private void Awake()
	{
		SettingCloseButton.onClick.RemoveAllListeners();

		SettingCloseButton.onClick.AddListener(() => CloseSettingPanel());
	}

	public void CloseSettingPanel()
	{
		SettingPanel.gameObject.SetActive(false);
	}

	public void PopupGameOverPanel()
	{
		GameOverPanel.gameObject.SetActive(true);
	}
}
