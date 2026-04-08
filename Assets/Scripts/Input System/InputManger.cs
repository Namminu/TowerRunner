using UnityEditor.TextCore.Text;
using UnityEngine;

public class InputManger : MonoBehaviour, IInitializable
{
	private void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButtonDown(0))
			TouchProcessor.Instance.ProcessTouchBegan(0, Input.mousePosition);
		if (Input.GetMouseButton(0))
			TouchProcessor.Instance.ProcessTouchMoved(0, Input.mousePosition);
		if (Input.GetMouseButtonUp(0))
			TouchProcessor.Instance.ProcessTouchEnded(0, Input.mousePosition);
#else
		foreach (var t in Input.touches)
		{
			switch (t.phase)
			{
				case TouchPhase.Began:
					TouchProcessor.Instance.ProcessTouchBegan(t.fingerId, t.position);
					break;

				//case TouchPhase.Moved:
				case TouchPhase.Stationary:
					TouchProcessor.Instance.ProcessTouchMoved(t.fingerId, t.position);
					break;

				//case TouchPhase.Ended:
				case TouchPhase.Canceled:
					TouchProcessor.Instance.ProcessTouchEnded(t.fingerId, t.position);
					break;
			}
		}
#endif
	}

	public void Init()
	{

	}
}
