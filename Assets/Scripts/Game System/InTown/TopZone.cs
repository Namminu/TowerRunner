using UnityEngine;
using UnityEngine.UI;

public class TopZone : MonoBehaviour, ISceneUI
{
	[SerializeField] private Text scoreText;
	[SerializeField] private Text goldText;

	public void InitUI()
	{
		int gold = SaveService.Current != null ? SaveService.Current.gold : 0;

		GameEvents.OnHighScoreChanged += ShowHighScore;
		GameEvents.OnGoldChanged += ShowCurrentPlayerGold;

		ShowCurrentPlayerGold(gold);
		//ShowHighScore(ScoreManager.Instance.HighScore);
		ShowHighScore(SaveService.Current.bestScore);
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
