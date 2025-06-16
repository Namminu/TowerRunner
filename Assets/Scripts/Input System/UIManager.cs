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
		// UI ������ Drag �� �������� ����
			// ���ܷ� ���� ���۹� ������ ���� �� ������?
		return;
	}

	public void OnDragEnd(Vector2 screenPos)
	{
		// UI ������ DragEnd �� �������� ����
		return;
	}
}
