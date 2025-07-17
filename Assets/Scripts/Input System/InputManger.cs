using UnityEditor.TextCore.Text;
using UnityEngine;

public class InputManger : MonoBehaviour
{
	private void Update()
	{
		#region ---- 마우스 입력 감지 ----
		if (Input.touchCount == 0)
		{
			if(Input.GetMouseButtonDown(0))
				TouchProcessor.Instance.ProcessTouchBegan(0, Input.mousePosition);
			if(Input.GetMouseButton(0))
				TouchProcessor.Instance.ProcessTouchMoved(0, Input.mousePosition);
			if(Input.GetMouseButtonUp(0))
				TouchProcessor.Instance.ProcessTouchEnded(0, Input.mousePosition);
		}
		#endregion
		#region ---- 모바일 터지 감지 ----
		else
		{
			foreach(var t in Input.touches)
			{
				switch(t.phase)
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
		}
		#endregion
	}

	public void Init()
	{

	}
}
