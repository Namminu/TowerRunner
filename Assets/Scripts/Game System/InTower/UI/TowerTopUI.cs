using UnityEngine;
using UnityEngine.UI;

public class TowerTopUI : MonoBehaviour
{
	[Header("Top Zone")]
	[SerializeField] private Text highScore;
	[SerializeField] private Text curScore;
	[SerializeField] private Button settingBtn;

	[Header("Panel Zone")]
	[SerializeField] private TowerPanelUI panelUI;

	private void Awake()
	{
	}

	public void SetHighScore(int _score)
	{
		highScore.text = _score.ToString();
	}

	private void SetCurScore(int _score)
	{
		curScore.text = _score.ToString();
	}

	private void OnDestroy()
	{
		settingBtn.onClick.RemoveAllListeners();
	}
}
