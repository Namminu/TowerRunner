using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemManager : MonoBehaviour, IInitializable
{
	public static ItemManager Instance { get; private set; }

	//private List<ItemDatabase.ItemEntry> _entries;
	private ItemDatabase _itemDB;
	private Action _runUnsub;

	private bool _isPrepared = false;
	private bool _runIssued = false;

	//private readonly List<Coroutine> _spawnCoroutines = new();
	private Coroutine _spawnCoroutine;

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		_runUnsub = GameBus.Subscribe<RunSignal>(OnRunSignal);

		GameStateManager.OnStateChanged += HandleGameStateChanged;
		GameEvents.OnPlayerFallInHole += HandlePlayerFallInHole;
	}

	private void HandleGameStateChanged(GameStateManager.GameState curState)
	{
		if (curState == GameStateManager.GameState.GameOver)
		{
			// 스폰 코루틴 정리
			//foreach (var c in _spawnCoroutines)
			//	if (c != null) StopCoroutine(c);
			//_spawnCoroutines.Clear();

			if (_spawnCoroutine != null)
			{
				StopCoroutine(_spawnCoroutine);
				_spawnCoroutine = null;
			}
		}
	}

	private void HandlePlayerFallInHole()
	{
		if (_spawnCoroutine != null)
		{
			StopCoroutine(_spawnCoroutine);
			_spawnCoroutine = null;
		}
	}

	public void SetData(ItemDatabase data)
	{
		//_entries = data.entries;
		_itemDB = data;
		_isPrepared = true;

		// 준비 완료 신호
		GameBus.Publish(new SubsystemReady(SubsystemId.Item));
		// 이미 RunSignal이 왔다면 즉시 스폰 시작
		if (_runIssued)
			StartSpawning();
	}

	private void OnRunSignal(RunSignal sig)
	{
		_runIssued = true;
		if (_isPrepared)
			StartSpawning();
	}

	private void StartSpawning()
	{
		// 중복 호출 방지
		//if (_spawnCoroutines.Count > 0) return;

		//foreach (var entry in _entries)
		//{
		//	var c = StartCoroutine(SpawnLoop(entry));
		//	_spawnCoroutines.Add(c);
		//}

		if (_spawnCoroutine != null) return;
		_spawnCoroutine = StartCoroutine(SpawnLoop());
	}

	//private IEnumerator SpawnLoop(ItemDatabase.ItemEntry entry)
	//{
	//	while(true)
	//	{
	//		float baseInterval = entry.spawnInterval;
	//		float speedMul = GameSpeedManager.Instance.SpeedMultiplier;
	//		float difficulty = Mathf.Lerp(1f, 2f, (Mathf.Clamp01(Time.timeSinceLevelLoad / 120f)));
	//		float interval = (baseInterval / speedMul) * difficulty;

	//		yield return new WaitForSeconds(interval);

	//		Vector3 spawnPos = GetRandomSpawnPosition();
	//		ItemPoolingManager.Instance.Spawn(entry.data, spawnPos);
	//	}
	//}

	private IEnumerator SpawnLoop()
	{
		WaitForSeconds errorWaitTime = new(1.0f);
		while (true)
		{
			float spawnInterval = _itemDB.GetRandomSpawnInterval();
			float adjustedInterval = spawnInterval / GameSpeedManager.Instance.SpeedMultiplier;
			yield return new WaitForSeconds(adjustedInterval);

			ItemData target = _itemDB.GetRandowmSpawnTarget();
			if (target == null)
			{
				Debug.LogError("Item Data Get Null Spawn Target");
				yield return errorWaitTime;
				continue;
			}
			else 
				ItemPoolingManager.Instance.Spawn(target, GetRandomSpawnPosition());
		}
	}

	private Vector3 GetRandomSpawnPosition()
	{
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 2, 0));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 2, 0));
		Vector3 pos = new(UnityEngine.Random.Range(leftTop.x + 1, rightTop.x - 1), leftTop.y + 1f, 0);

		return pos;
	}

	private void OnDestroy()
	{
		// 구독 해제
		_runUnsub?.Invoke();

		// 스폰 코루틴 정리
		//foreach (var c in _spawnCoroutines)
		//	if (c != null) StopCoroutine(c);
		//_spawnCoroutines.Clear();

		if (_spawnCoroutine != null)
		{
			StopCoroutine(_spawnCoroutine);
			_spawnCoroutine = null;
		}

		GameStateManager.OnStateChanged -= HandleGameStateChanged;
		GameEvents.OnPlayerFallInHole -= HandlePlayerFallInHole;
	}
}
