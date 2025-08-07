using UnityEngine;

public class PlayerItemChecker : MonoBehaviour
{
	[SerializeField]
	private PlayerEffectChecker effectChecker;
	[SerializeField]
	private CoinMagnetController magnetCtrl;
	[SerializeField]
	private Player player;

	private void Awake()
	{
		if(effectChecker == null)
			effectChecker = GetComponent<PlayerEffectChecker>();
		if (magnetCtrl == null)
			magnetCtrl = transform.Find("CoinCollider").GetComponent<CoinMagnetController>();
		if(player == null)
			player = GetComponent<Player>();

		player.OnShieldConsumed += () =>
		{
			effectChecker.ShieldEffect(false);
		};
	}

	#region Health Potion Item
	public void Heal(float amount)
	{
		Debug.Log("Player Heal! " + amount);
		player.PlayerCurHealth += amount;
		effectChecker.HealingEffect();
	}
	#endregion

	#region Battle Booster Item
	public void ApplyBattleBooster()
	{
		Debug.Log("Player Battle Booster On!");
		player.SetInvincible(true);
		effectChecker.BattleBoosterEffect(true);
	}

	public void RemoveBattleBooster()
	{
		Debug.Log("Player Battle Booster Off..");
		player.SetInvincible(false);
		effectChecker.BattleBoosterEffect(false);
	}
	#endregion

	#region Shield Item
	public void GetShield()
	{
		Debug.Log("Player Shield On!");
		player.SetShieldOn();
		effectChecker.ShieldEffect(true);
	}
	#endregion

	#region Coin Item
	public void GetCoin(int amount)
	{
		Debug.Log("Player Get Coin! : " + amount);
		player.PlayerGold += amount;
		effectChecker.DropCoinEffect();
	}
	#endregion

	#region Coin Magnet Item
	public void ApplyCoinMagnet(float range)
	{
		Debug.Log("Player Coin Magnet On!");
		magnetCtrl.ActivateMagnet(range);
		effectChecker.CoinMagnetEffect(true);
	}

	public void RemoveCoinMagnet()
	{
		Debug.Log("Player Coin Magnet Off..");
		magnetCtrl.DeactiveMagnet();
		effectChecker.CoinMagnetEffect(false);
	}
	#endregion

	#region Fatal Elixir Item
	public void ApplyFatalElixir(int inhance)
	{
		Debug.Log("Player Fatal Elixir On!");
		player.PlayerFatalRate += inhance;
		effectChecker.FatalElixirEffect(true);
	}

	public void RemoveFatalElixir(int inhance)
	{
		Debug.Log("Player Fatal Elixir Off..");
		player.PlayerFatalRate -= inhance;
		effectChecker.FatalElixirEffect(false);
	}
	#endregion
}
