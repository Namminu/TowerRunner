using System;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
	[SerializeField, Tooltip("Object Fall Down Speed Range, Min : 1f"), Range(2f, 5f)]
	private float moveSpeed = 2f;
	public float MoveDownSpeed => moveSpeed;

	private float _lowerBoundY;
	private float _speedIncrease;
	
	private Renderer _rd;
	private float _objBoundY;

	public event Action<ObjectMover> OnOutofBounds;

	private void Awake()
	{
		_lowerBoundY = ScreenBounds.LowerY;
		_rd = GetComponent<Renderer>();
		if( _rd == null)
			Debug.Log(this + " has no Renderer");

		_objBoundY = _rd.bounds.size.y;
	}
	 
	private void Start()
	{
		_speedIncrease = GameSpeedManager.Instance.SpeedMultiplier;
	}

	private void Update()
	{
		transform.position += moveSpeed * _speedIncrease * Time.deltaTime * Vector3.down;
		if (transform.position.y + (_objBoundY * 0.5f) < _lowerBoundY)
		{
			Debug.Log(name + " Out of Bound!");
			OnOutofBounds?.Invoke(this);
		}
	}
}
