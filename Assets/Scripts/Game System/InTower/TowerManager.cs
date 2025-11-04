using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum SubsystemId
{
	Map,
	Enemy,
	Item,
	Projectile,
	Speed,
	Audio,

}

public class TowerManager : MonoBehaviour, IInitializable
{
	[Header("Ready / Run UI Setup")]
	[SerializeField, Tooltip("Run 문구 최소 대기 시간")] 
	private float _runStateMinTime = 1.0f;
	private bool _minDelayPassed = false;

	[SerializeField]
	private List<SubsystemId> requiredSubsystems = new();
	private HashSet<SubsystemId> _readySet = new();
	private bool _allReady = false;
	private bool _isRunIssued = false;

	private TowerStateTextUI _textUI;
	private readonly List<Action<TowerStateTextUI>> _pendingUIActions = new();

	// TowerScene 신호 제어
	private Action _towerUI;
	private Action _subsystemReady;
	private Action _subsystemFailed;

	public void Init()
	{
		// 신호 구독
		_towerUI = GameBus.Subscribe<StateUIReady>(OnTowerUIReady);
		_subsystemReady = GameBus.Subscribe<SubsystemReady>(OnSubsystemReady);
		_subsystemFailed = GameBus.Subscribe<SubsystemFailed>(OnSubSystemFailed);

		// 자식 매니저 오브젝트 초기화 호출
		IInitializable[] inits = GetComponentsInChildren<IInitializable>(true);
		foreach (var init in inits)
		{
			if (!ReferenceEquals(init, this))
				init.Init();
		}

		// Ready 최소 표시 시간 카운트
		StartCoroutine(ReadyMinDelayRoutine());

		// AllReady 체크
		StartCoroutine(AllReadyCheck());
	}

	private IEnumerator ReadyMinDelayRoutine()
	{
		float t = Time.unscaledTime;
		while (Time.unscaledTime - t < _runStateMinTime)
			yield return null;
		_minDelayPassed = true;
	}

	private void OnTowerUIReady(StateUIReady sig)
	{
		_textUI = sig.TextUI;

		if (_pendingUIActions.Count > 0)
		{
			foreach (var act in _pendingUIActions)
				act(_textUI);
			_pendingUIActions.Clear();
		}
	}

	private void OnSubsystemReady(SubsystemReady sig)
	{ 
		if(requiredSubsystems.Contains(sig.Id))
		{
			_readySet.Add(sig.Id);
		}
	}

	private void OnSubSystemFailed(SubsystemFailed sig)
	{
		Debug.LogError($"[Tower Manager] Subsystem {sig.Id} Failed : {sig.Reason}");
	}

	private IEnumerator AllReadyCheck()
	{
		while(!_isRunIssued)
		{
			bool allListendReady = _readySet.Count == requiredSubsystems.Count;
			_allReady = allListendReady;
			// 모든 준비 동작 체크 완료 시
			if (_minDelayPassed && _allReady)
			{
				// Run 연출
				_textUI.StartRunState();

				GameBus.Publish(new RunSignal());
				yield return null;
				GameBus.Publish(new ActivateMovementNextFrame());

				_isRunIssued = true;
			}
			// 아니라면 준비 동작 대기
			yield return null;
		}
	}

	private void OnDestroy()
	{
		// 구독 해제
		_towerUI?.Invoke();
		_subsystemReady?.Invoke();
		_subsystemFailed?.Invoke();
	}
}
