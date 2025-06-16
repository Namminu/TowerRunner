using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	#region ---- Members ----
	private float speed = 0f;
	private Transform playerTr;
	#endregion

	private void Awake()
	{
		playerTr = gameObject.GetComponent<Transform>();
	}

	public void OnTap(Vector2 screenPos)
	{
		Debug.Log("Player Tap");
		
	}

	public void OnDrag(Vector2 delta)
	{
		Debug.Log("Player Dragging");
		Vector3 worldDelta = Camera.main.ScreenToWorldPoint(delta) - Camera.main.ScreenToWorldPoint(Vector2.zero);
		playerTr.position += new Vector3(worldDelta.x, 0, 0) * speed;
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
