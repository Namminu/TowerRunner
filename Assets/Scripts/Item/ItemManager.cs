using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour, IInitializable
{
	public static ItemManager Instance { get; private set; }

	private List<ItemDatabase.ItemEntry> _entries;
	private Action _runUnsub;

	private bool _isPrepared = false;
	private bool _runIssued = false;

	private readonly List<Coroutine> _spawnCoroutines = new();

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		_runUnsub = GameBus.Subscribe<RunSignal>(OnRunSignal);

		GameStateManager.OnStateChanged += HandleGameStateChanged;
	}

	private void HandleGameStateChanged(GameStateManager.GameState curState)
	{
		if (curState == GameStateManager.GameState.GameOver)
		{
			// 스폰 코루틴 정리
			foreach (var c in _spawnCoroutines)
				if (c != null) StopCoroutine(c);
			_spawnCoroutines.Clear();
		}
	}

	public void SetData(ItemDatabase data)
	{
		_entries = data.entries;
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
		if (_spawnCoroutines.Count > 0) return;

		foreach (var entry in _entries)
		{
			var c = StartCoroutine(SpawnLoop(entry));
			_spawnCoroutines.Add(c);
		}
	}

	private IEnumerator SpawnLoop(ItemDatabase.ItemEntry entry)
	{
		while(true)
		{
			float baseInterval = entry.spawnInterval;
			float speedMul = GameSpeedManager.Instance.SpeedMultiplier;
			float difficulty = Mathf.Lerp(1f, 2f, (Mathf.Clamp01(Time.timeSinceLevelLoad / 120f)));
			float interval = (baseInterval / speedMul) * difficulty;

			yield return new WaitForSeconds(interval);

			Vector3 spawnPos = GetRandomSpawnPosition();
			ItemPoolingManager.Instance.Spawn(entry.data, spawnPos);
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
		foreach (var c in _spawnCoroutines)
			if (c != null) StopCoroutine(c);
		_spawnCoroutines.Clear();

		GameStateManager.OnStateChanged -= HandleGameStateChanged;
	}
}
