using System;
using System.Collections;
using UnityEngine;

public class GameSpeedManager : MonoBehaviour, IInitializable
{
	public static GameSpeedManager Instance { get; private set; }
	public float SpeedMultiplier { get; private set; } = 1f;

	[Tooltip("speed increase per second during combat")]
	[SerializeField, Range(0.01f, 1f)]
	private float speedAccelrator = 0.01f;

	private bool _isRunning = false;
	private Action _unsubRun;
	private Action _unsubActivate;

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		StartCoroutine(InitRoutine());
		Debug.Log("Is Speed Manager Init Called?");
	}

	private IEnumerator InitRoutine()
	{
		yield return StartCoroutine(SubscribeRoutine());
		// 준비 완료 시그널 전송
		GameBus.Publish(new SubsystemReady(SubsystemId.Speed));
	}

	private IEnumerator SubscribeRoutine()
	{
		yield return null;
		// 초기엔 update 비활성화
		enabled = false;
		// Run 상태 준비
		_unsubRun = GameBus.Subscribe<RunSignal>(_ => { _isRunning = false; enabled = false; });
		// Run 상태 시작
		_unsubActivate = GameBus.Subscribe<ActivateMovementNextFrame>(_ =>
		{
			_isRunning = true;
			enabled = true;
		});
	}

	private void Update()
	{
		if (!_isRunning) return;
		SpeedMultiplier = 1f + Time.timeSinceLevelLoad * speedAccelrator;
	}

	private void OnDestroy()
	{
		// 구독 해제
		_unsubRun?.Invoke();
		_unsubActivate?.Invoke();
	}
}
