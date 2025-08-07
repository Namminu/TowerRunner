using System;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
	[SerializeField, Tooltip("Object Fall Down Speed Range, Min : 1f"), Range(2f, 5f)]
	private float maxSpeedRange;

	private float _speed;
	public float MoveDownSpeed => _speed;

	private float lowerBoundY;
	private float speedIncrease;

	public event Action<ObjectMover> OnOutofBounds;

	private void Awake()
	{
		lowerBoundY = ScreenBounds.LowerY;
		_speed = UnityEngine.Random.Range(1f, maxSpeedRange);
	}

	private void Start()
	{
		speedIncrease = GameSpeedManager.Instance.SpeedMultiplier;
	}

	private void Update()
	{
		transform.position += Vector3.down * _speed * speedIncrease * Time.deltaTime;
		if (transform.position.y < lowerBoundY)
		{
			Debug.Log(name + "Out of Bound!");
			OnOutofBounds?.Invoke(this);
		}
	}
}
