using UnityEngine;

public class PlayerItemChecker : MonoBehaviour
{
	[SerializeField]
	private PlayerEffectChecker effectChecker;
	[SerializeField]
	private CoinMagnetController magnetCtrl;

	private void Awake()
	{
		if(effectChecker == null)
			effectChecker = GetComponent<PlayerEffectChecker>();
		if (magnetCtrl == null)
			magnetCtrl = transform.Find("CoinCollider").GetComponent<CoinMagnetController>();

	}


}
