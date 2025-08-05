using UnityEngine;
using UnityEngine.UI;

public class TopZone : MonoBehaviour, ISceneUI
{
	[SerializeField] private Text scoreText;
	[SerializeField] private Text goldText;

	public void InitUI()
	{
		ShowHighScore(ScoreManager.Instance.HighScore);
		ShowCurrentPlayerGold(Player.Instance.PlayerGold);

		GameEvents.OnHighScoreChanged += ShowHighScore;
		GameEvents.OnGoldChanged += ShowCurrentPlayerGold;
	}

	private void ShowHighScore(int highScore)
	{
		scoreText.text = highScore.ToString();
	}

	private void ShowCurrentPlayerGold(int gold)
	{
		goldText.text = gold.ToString();
	}

	private void OnDestroy()
	{
		GameEvents.OnHighScoreChanged -= ShowHighScore;
		GameEvents.OnGoldChanged -= ShowCurrentPlayerGold;
	}
}
