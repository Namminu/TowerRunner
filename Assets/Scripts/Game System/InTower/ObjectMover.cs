using System;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
	public enum MoveType { Envi, Entity }
	[Header("Object Move Type")]
	[SerializeField] private MoveType moveType;

	[SerializeField, Tooltip("Object Fall Down Speed Range, Min : 1f"), Range(2f, 5f)]
	private float moveSpeed = 2f;
	public float MoveDownSpeed => moveSpeed;

	private float _lowerBoundY;
	
	private Renderer _rd;
	private float _objBoundY;

	public event Action<ObjectMover> OnOutofBounds;

	private bool _isRunning = false;
	private bool _isPaused = true;

	private Action _unsubRun;
	private Action _unsubActivate;

	private void Awake()
	{
		_lowerBoundY = ScreenBounds.LowerY;
		_rd = GetComponentInChildren<Renderer>();
		if( _rd == null)
			Debug.Log(this + " has no Renderer");

		_objBoundY = _rd.bounds.size.y;

		// Run 상태 준비
		_unsubRun = GameBus.SubscribeSticky<RunSignal>(_ => { _isRunning = false; _isPaused = true;  enabled = false; });
		// Run 상태 시작
		_unsubActivate = GameBus.SubscribeSticky<ActivateMovementNextFrame>(_ =>
		{
			_isRunning = true;
			_isPaused = false;
			enabled = true;
		});

		enabled = false;
	}

	private void OnEnable()
	{
		GameStateManager.OnStateChanged += HandleGameStateChanged;
	}

	private void Update()
	{
		if (!_isRunning || _isPaused) return;

		float finalSpeed = 1f;

		if(moveType == MoveType.Envi)
		{
			finalSpeed = GameSpeedManager.Instance.EnviSpeed;
		}
		else if(moveType == MoveType.Entity)
		{
			finalSpeed = GameSpeedManager.Instance.SpeedMultiplier;
		}

		transform.position += moveSpeed * finalSpeed * Time.deltaTime * Vector3.down;

		if (transform.position.y + (_objBoundY * 0.5f) < _lowerBoundY)
		{
			OnOutofBounds?.Invoke(this);
		}
	}

	public void PauseMovement() => _isPaused = true;
	public void ResumeMovement() => _isPaused = false;

	private void OnDestroy()
	{
		_unsubRun?.Invoke();
		_unsubActivate?.Invoke();
	}

	private void OnDisable()
	{
		GameStateManager.OnStateChanged -= HandleGameStateChanged;
	}

	private void HandleGameStateChanged(GameStateManager.GameState curState)
	{
		switch (curState)
		{
			case GameStateManager.GameState.Play:
				_isRunning = true;
				_isPaused = false;
				enabled = true;
				break;

			case GameStateManager.GameState.GameOver:
				_isRunning = false;
				_isPaused = true;
				enabled = false;
				break;
		}
	}
}
