using UnityEngine;

public class TownManager : MonoBehaviour, IInitializable
{
	public void Init()
	{
		AudioManager.Instance.PlayBGM(AudioID.TownBGM);
		Player.Instance.gameObject.SetActive(false);
	}
}
