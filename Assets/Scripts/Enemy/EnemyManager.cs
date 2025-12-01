using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour, IInitializable
{
	public static EnemyManager Instance { get; private set; }

	private List<EnemyData.EnemyEntry> _entries;
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
		// RunSignal 구독: RunSignal 수신 시 스폰 시작
		_runUnsub = GameBus.Subscribe<RunSignal>(OnRunSignal);
	}

	public void SetData(EnemyData data)
	{
		_entries = data.entries;
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
		// 중복 호출 방지
		if (_spawnCoroutines.Count > 0) return;

		foreach (var entry in _entries)
		{
			var c = StartCoroutine(SpawnLoop(entry));
			_spawnCoroutines.Add(c);
		}
	}

	private IEnumerator SpawnLoop(EnemyData.EnemyEntry entry)
	{
		while(true)
		{
			float waitTime = entry.spawnInterval / GameSpeedManager.Instance.SpeedMultiplier;
			yield return new WaitForSeconds(waitTime);

			EnemyPoolingManager.Instance.Spawn(entry.prefabRef, GetSpawnPosition(), Quaternion.identity);
		}
	}

	private Vector3 GetSpawnPosition()
	{
		float z = Mathf.Abs(Camera.main.transform.position.z);
		var leftTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 2, z));
		var rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1, 2, z));
		Vector3 pos = new(UnityEngine.Random.Range(leftTop.x + 1, rightTop.x -1), leftTop.y + 1f, 0);

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
	}
}
