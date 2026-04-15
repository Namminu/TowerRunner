using UnityEngine;

public class TownManager : MonoBehaviour, IInitializable
{
	public void Init()
	{
		Player.Instance.gameObject.SetActive(false);
	}
}
