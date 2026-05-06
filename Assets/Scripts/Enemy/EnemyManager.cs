using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.EventSystems.EventTrigger;


public class EnemyManager : MonoBehaviour, IInitializable
{
	public static EnemyManager Instance { get; private set; }

	//private List<EnemyData.EnemyEntry> _entries;
	private EnemyData enemyData;
	private Action _runUnsub;

	private bool _isPrepared = false;
	private bool _runIssued = false;

	//private readonly List<Coroutine> _spawnCoroutines = new();
	private Coroutine _spawnCoroutines;

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		// RunSignal 구독: RunSignal 수신 시 스폰 시작
		_runUnsub = GameBus.Subscribe<RunSignal>(OnRunSignal);

		GameStateManager.OnStateChanged += HandleGameStateChanged;
		GameEvents.OnPlayerFallInHole += HandlePlayerFallInHole;
	}

	public void SetData(EnemyData data)
	{
		//_entries = data.entries;
		enemyData = data;
		_isPrepared = true;

		// 준비 완료 신호
		GameBus.Publish(new SubsystemReady(SubsystemId.Enemy));
		// 이미 RunSignal이 왔다면 즉시 스폰 시작
		if (_runIssued)
			StartSpawning();
	}

	private void OnRunSignal(RunSignal sig)
	{
		_runIssued = true;
		if(_isPrepared)
			StartSpawning();
	}

	private void StartSpawning()
	{
		//// 중복 호출 방지
		//if (_spawnCoroutines.Count > 0) return;
		//foreach (var entry in _entries)
		//{
		//	var c = StartCoroutine(SpawnLoop(entry));
		//	_spawnCoroutines.Add(c);
		//}

		if (_spawnCoroutines != null) return;
		_spawnCoroutines = StartCoroutine(SpawnLoop());

	}

	//private IEnumerator SpawnLoop(EnemyData.EnemyEntry entry)
	//{
	//	while(true)
	//	{
	//		float waitTime = entry.spawnInterval / GameSpeedManager.Instance.SpeedMultiplier;
	//		yield return new WaitForSeconds(waitTime);

	//		EnemyPoolingManager.Instance.Spawn(entry.prefabRef, GetSpawnPosition(), Quaternion.identity);
	//	}
	//}

	private IEnumerator SpawnLoop()
	{
		WaitForSeconds errorWaitTime = new(1.0f);
		while (true)
		{
			float spawnInterval = enemyData.GetRandomSpawnInterval();
			float adjustedInterval = spawnInterval / GameSpeedManager.Instance.SpeedMultiplier;
			yield return new WaitForSeconds(adjustedInterval);

			AssetReferenceGameObject target = enemyData.GetRandomSpawnTarget();
			if (target == null)
			{
				Debug.LogError("Enemy Data Get Null Spawn Target");
				yield return errorWaitTime;
				continue;
			}
			EnemyPoolingManager.Instance.Spawn(target, GetSpawnPosition(), Quaternion.identity);
		}
	}


	private Vector3 GetSpawnPosition()
	{
		float z = Camera.main.transform.position.z;
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 2, 0));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 2, 0));

		Vector3 pos = new(UnityEngine.Random.Range(leftTop.x + 1, rightTop.x -1), leftTop.y + 1f, 0);

		return pos;
	}

	private void HandleGameStateChanged(GameStateManager.GameState curState)
	{
		if(curState == GameStateManager.GameState.GameOver)
		{
			// 스폰 코루틴 정리
			//foreach (var c in _spawnCoroutines)
			//	if (c != null) StopCoroutine(c);
			//_spawnCoroutines.Clear();

			if(_spawnCoroutines != null) StopCoroutine(_spawnCoroutines);
			 _spawnCoroutines = null;
		}
	}

	private void HandlePlayerFallInHole()
	{
		if (_spawnCoroutines != null) StopCoroutine(_spawnCoroutines);
		_spawnCoroutines = null;
	}

	private void OnDestroy()
	{
		// 구독 해제
		_runUnsub?.Invoke();

		// 스폰 코루틴 정리
		//foreach (var c in _spawnCoroutines)
		//	if (c != null) StopCoroutine(c);
		//_spawnCoroutines.Clear();

		if (_spawnCoroutines != null) StopCoroutine(_spawnCoroutines);
		_spawnCoroutines = null;

		GameStateManager.OnStateChanged -= HandleGameStateChanged;
		GameEvents.OnPlayerFallInHole -= HandlePlayerFallInHole;
	}
}
