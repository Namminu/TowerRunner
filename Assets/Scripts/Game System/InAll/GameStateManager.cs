using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateManager : MonoBehaviour, IInitializable
{
	public static GameStateManager Instance { get; private set; }

	public enum GameState { Play, Pause, GameOver }

	public GameState CurState { get; private set; }

	public static event Action<GameState> OnStateChanged;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Init()
	{
		SetState(GameState.Play);
	}

	public void SetState(GameState newState)
	{
		CurState = newState;

		// 설정창에 의한 일시정지
		if(CurState == GameState.Pause)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}

		OnStateChanged?.Invoke(CurState);
	}
}
