using UnityEngine;
using UnityEngine.UI;

public class TowerSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Score")]
	[SerializeField] private Text HighScore;
	[SerializeField] private Text CurScore;

	[Header("Setting")]
	[SerializeField] private Button SettingBtn;
	[SerializeField] private GameObject Panel;

	public void InitUI()
	{
		Debug.Log("Tower Scene UI Init Called");

		SettingBtn.onClick.RemoveAllListeners();
		SettingBtn.onClick.AddListener(() => TogglePanel());
	}

	private void TogglePanel()
	{
		Panel.SetActive(true);
	}
}
