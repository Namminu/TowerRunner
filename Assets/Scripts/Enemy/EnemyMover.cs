using System;
using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyMover : MonoBehaviour
{
	[SerializeField]
	EnemyData datas;

	private float lowerBoundY;
	private float moveDownSpeed;

	private float speedIncrease;

	public event Action<EnemyMover> OnOutofBounds;

	private void Awake()
	{
		moveDownSpeed = datas.moveDownSpeed;
		lowerBoundY = ScreenBounds.LowerY;
		speedIncrease = GameSpeedManager.Instance.SpeedMultiplier;
	}

	private void Update()
	{
		transform.position += Vector3.down * moveDownSpeed * speedIncrease * Time.deltaTime;
		if (transform.position.y < lowerBoundY)
		{
			Debug.Log(this.name + "Out of Bound!");
			OnOutofBounds?.Invoke(this);
		}
			
	}
}
