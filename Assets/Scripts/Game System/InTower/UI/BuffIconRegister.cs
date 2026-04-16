using UnityEngine;

public class BuffIconRegister : MonoBehaviour
{
	private void Start()
	{
		if(TimerUIManager.Instance != null)
		{
			TimerUIManager.Instance.RegisterParent(transform);
		}
	}
}
