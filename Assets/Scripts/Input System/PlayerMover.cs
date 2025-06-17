using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	#region ---- Members ----
	private float speed = 0f;
	private Transform playerTr;

	private float halfWidth;
	private float minX, maxX;
	private float zDist;
	#endregion

	private void Awake()
	{
		playerTr = gameObject.GetComponent<Transform>();

		var sr = GetComponent<SpriteRenderer>();
		if (sr != null)
			halfWidth = sr.bounds.extents.x;
		else halfWidth = 0.5f;

		zDist = Mathf.Abs(Camera.main.transform.position.z - playerTr.position.z);
		Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, zDist));
		Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, zDist));
	
		minX = worldLeft.x + halfWidth;
		maxX = worldRight.x - halfWidth;
	}

	//public void OnTap(Vector2 screenPos)
	//{
	//	return;
	//}

	public void OnDrag(Vector2 delta)
	{
		Debug.Log("Player Dragging");

		Vector3 screenDelta = new Vector3(delta.x, delta.y, zDist);
		Vector3 worldDelta = Camera.main.ScreenToWorldPoint(screenDelta)
							- Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zDist));
		playerTr.position += new Vector3(worldDelta.x, 0, 0) * speed * Time.deltaTime;

		float clampedX = Mathf.Clamp(playerTr.position.x, minX, maxX);
		playerTr.position = new Vector3(clampedX, playerTr.position.y, playerTr.position.z);
	}

	public void OnDragEnd(Vector2 screenPos)
	{
		Debug.Log("Player Drag End");

	}

	public void SetSpeed(float _speed)
	{
		speed = _speed;
	}
}
