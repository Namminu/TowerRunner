using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static ScoreManager;

public class EndGamePanelUI : MonoBehaviour
{
	[System.Serializable]
	public struct ScoreSet
	{
		public Text labelText;
		public Text scoreText;
	}
	[SerializeField] private ScoreSet[] scoreSets;
	[SerializeField] private float delayBetweenSteps = 0.5f;

	[Header("Button")]
    [SerializeField] private Button CloseButton;

	private FinalBreakdown ScoreStruct;

	private void OnEnable()
	{
		foreach(var scoreSet in scoreSets)
		{
			scoreSet.labelText.gameObject.SetActive(false);
			scoreSet.scoreText.gameObject.SetActive(false);
		}

		OnSessionEnded += HandleSessionEnded;

		CloseButton.onClick.RemoveAllListeners();
		CloseButton.onClick.AddListener(() => CloseGameOverPanel());
	}

	private void HandleSessionEnded(FinalBreakdown breakdown)
	{
		ScoreStruct = breakdown;
		HandleGameEndCheck();
	}

	public void CloseGameOverPanel()
	{
		// Town Scene 이동
		StartCoroutine(GameOverRoutine());
	}

	private IEnumerator GameOverRoutine()
	{
		yield return ManagersInitializer.Instance.SceneLoadRoutine(Scenes.Town);
	}

	public void HandleGameEndCheck()
	{
		StartCoroutine(GameEndRoutine());
	}

	private IEnumerator GameEndRoutine()
	{
		yield return new WaitForSeconds(1.5f); // 잠시 대기 후 시작

		WaitForSeconds waitTime = new(delayBetweenSteps);

		for (int i = 0; i < scoreSets.Length; i++)
		{
			scoreSets[i].labelText.gameObject.SetActive(true);

			int scoreValue = i switch
			{
				0 => ScoreStruct.DistanceScore,
				1 => ScoreStruct.MonsterKillScore,
				2 => ScoreStruct.ItemUseScore,
				3 => ScoreStruct.NoHitScore,
				4 => ScoreStruct.TotalScore,
				_ => 0
			};
			scoreSets[i].scoreText.text = scoreValue.ToString();
			scoreSets[i].scoreText.gameObject.SetActive(true);

			yield return waitTime;
		}
	}

	private void OnDisable()
	{
		CloseButton.onClick.RemoveAllListeners();
	}
}
