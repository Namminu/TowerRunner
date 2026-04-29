using System;
using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using static ScoreManager;

public class ScoreManager : MonoBehaviour, IInitializable
{
	public static ScoreManager Instance { get; private set; }

	[Header("Time Score")]
	[SerializeField, Tooltip("Score Per Second")]
	private float secondScore = 10f;

	[Header("Bonus Score")]
	[SerializeField, Tooltip("Monster 1 Kill Score")]
	private int monsterKillBonus = 50;
	[SerializeField, Tooltip("Item 1 Use Score")]
	private int itemUserBonus = 10;
	[SerializeField, Tooltip("No Hitted Score")]
	private int noHitBonus = 100;

	private Coroutine sessionRoutine;

	private int currentScore;
	public int CurrentScore => currentScore;

	private int highScore;
	public int HighScore => highScore;

	private struct RawBreakdown
	{
		public int DistanceScore;
		public int MonsterKillCount;
		public int ItemUseCount;
		public bool IsPlayerHit;
	} private RawBreakdown raw;

	public struct FinalBreakdown
	{
		public int DistanceScore;
		public int MonsterKillScore;
		public int ItemUseScore;
		public int NoHitScore;

		public int TotalScore;
	}
	//public static event Action<FinalBreakdown> OnSessionEnded;
	private FinalBreakdown finalBreakdown;
	public FinalBreakdown FinalBreakDown => finalBreakdown;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		highScore = SaveService.Current.bestScore;
	}

	private void Start()
	{
		GameEvents.RaiseHighScoreChanged(0);
	}

	private void StartSession()
	{
		if(sessionRoutine != null)
			StopCoroutine(sessionRoutine);

		raw = default;
		currentScore = 0;
		GameEvents.RaiseScoreChanged(highScore);

		sessionRoutine = StartCoroutine(SessionTimerTicker());
	}

	private void EndSession()
	{
		if (sessionRoutine != null)
		{
			StopCoroutine(sessionRoutine);
			sessionRoutine = null;
		}

		int noHitScore = raw.IsPlayerHit ? 0 : noHitBonus;
		int monsterScore = raw.MonsterKillCount * monsterKillBonus;
		int itemScore = raw.ItemUseCount * itemUserBonus;

		int total = raw.DistanceScore + monsterScore + itemScore + noHitScore;

		if(total > highScore)
		{
			highScore = total;
			GameEvents.RaiseHighScoreChanged(highScore);

			SaveService.Current.bestScore = highScore;
		}

		finalBreakdown = new FinalBreakdown
		{
			DistanceScore = raw.DistanceScore,
			MonsterKillScore = monsterScore,
			ItemUseScore = itemScore,
			NoHitScore = noHitScore,
			TotalScore = total
		};

		//OnSessionEnded?.Invoke(new FinalBreakdown
		//{
		//	DistanceScore = raw.DistanceScore,
		//	MonsterKillScore = monsterScore,
		//	ItemUseScore = itemScore,
		//	NoHitScore = noHitScore,
		//	TotalScore = total
		//});
	}

	private IEnumerator SessionTimerTicker()
	{
		float acc = 0f;
		while(true)
		{
			acc += secondScore * Time.deltaTime * GameSpeedManager.Instance.EnviSpeed;
			int delta = Mathf.FloorToInt(acc);
			if(delta > 0)
			{
				acc -= delta;
				raw.DistanceScore += delta;
				AddScore(delta);
			}
			yield return null;
		}
	}

	public void Init()
	{
		GameEvents.OnBattleStarted += StartSession;
		GameEvents.OnBattleEnded += EndSession;
	}

	/// <summary>
	/// Check Count Player Kill Monster
	/// </summary>
	public void RegisterMonsterKill()
	{
		raw.MonsterKillCount++;
		int pts = monsterKillBonus;
		AddScore(pts);
	}

	/// <summary>
	/// Check Count Player Use Item
	/// </summary>
	public void RegisterItemUse()
	{
		raw.ItemUseCount++;
		int pts = itemUserBonus;
		AddScore(pts);
	}

	/// <summary>
	/// Check Is Player Hitted
	/// </summary>
	public void RegisterPlayerHit()
	{
		raw.IsPlayerHit = true;
	}

	public void AddScore(int amount)
	{
		if (amount <= 0) return;
		currentScore += amount;
		GameEvents.RaiseScoreChanged(currentScore);
	}

	public void ResetScore()
	{
		currentScore = 0;
		GameEvents.RaiseScoreChanged(currentScore);
	}

	private void OnDestroy()
	{
		GameEvents.OnBattleStarted -= StartSession;
		GameEvents.OnBattleEnded -= EndSession;
	}
}