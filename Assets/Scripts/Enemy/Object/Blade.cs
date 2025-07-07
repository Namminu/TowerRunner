using UnityEngine;

public class Blade : BaseObject
{
	[SerializeField, Tooltip("Self Rotating Speed")]
	private float zRotateSpeed;

	[SerializeField, Tooltip("Side Round Tripping Speed")]
	private float sideMoveSpeed;

	private float minX, maxX;
	private int direction;

	private void Awake()
	{
		direction = 1;
	}

	private void Start()
	{
		minX = ScreenBounds.LeftX;
		maxX = ScreenBounds.RightX;
	}

	private void Update()
	{
		/* Rotation Z Anchor at Self Space */
		transform.Rotate(new Vector3(0, 0, zRotateSpeed * GameSpeedManager.Instance.SpeedMultiplier) 
			* Time.deltaTime, Space.Self);

		/* Side Move in Screen Bound */
		var pos = transform.position;
		pos.x += sideMoveSpeed * direction * Time.deltaTime;
		if(pos.x > maxX)
		{
			pos.x = maxX;
			direction *= -1;
		}
		else if(pos.x < minX)
		{
			pos.x = minX;
			direction *= -1;
		}
		transform.position = pos;
	}
}
