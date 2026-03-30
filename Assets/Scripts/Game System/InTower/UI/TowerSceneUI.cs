using UnityEngine;
using UnityEngine.UI;

public class TowerSceneUI : MonoBehaviour, ISceneUI
{
	[Header("Top Zone")]
	[SerializeField] private TowerTopUI TopUI;

	public void InitUI()
	{
		Debug.Log("Tower Scene UI Init Called");

		if(TopUI)
		{
			TopUI.InitUI();
		}
	}
}
