using UnityEngine;
using UnityEngine.UI;

public class TowerTopUI : MonoBehaviour
{
	[Header("Top Zone")]
	[SerializeField] private Text highScore;
	[SerializeField] private Text curScore;

	[Header("Panel Zone")]
	[SerializeField] private Button settingBtn;
	[SerializeField] private TowerPanelUI PanelUI;

	public void InitUI()
	{
		settingBtn.onClick.RemoveAllListeners();
		settingBtn.onClick.AddListener(() => ShowSettingPanel());

		SetHighScore(SaveService.Current.bestScore);

		GameEvents.OnScoreChanged += SetCurScore;
	}

	private void ShowSettingPanel()
	{
		AudioManager.Instance.PlaySound(AudioID.ButtonClick);
		GameStateManager.Instance.SetState(GameStateManager.GameState.Pause);
		AudioManager.Instance.PauseBGM();

		PanelUI.ShowPanel(TowerPanelType.Setting);
	}

	private void SetHighScore(int _score)
	{
		highScore.text = _score.ToString();
	}

	private void SetCurScore(int _score)
	{
		curScore.text = _score.ToString();
	}

	private void OnDisable()
	{
		GameEvents.OnScoreChanged -= SetCurScore;
		settingBtn.onClick.RemoveAllListeners();
	}
}
