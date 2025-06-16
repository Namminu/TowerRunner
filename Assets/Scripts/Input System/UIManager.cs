using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance {  get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void OnTap(Vector2 screenPos)
	{
		Debug.Log("UI Tap");
	}

	public void OnDrag(Vector2 screenPos)
	{
		// UI 에서의 Drag 는 동작하지 않음
			// 예외로 사운드 조작바 같은건 있을 수 있을듯?
		return;
	}

	public void OnDragEnd(Vector2 screenPos)
	{
		// UI 에서의 DragEnd 는 동작하지 않음
		return;
	}
}
