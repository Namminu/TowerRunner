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
			player = GetComponentInParent<Player>();

		player.OnShieldConsumed += () =>
		{
			effectChecker.ShieldEffect(false);
		};
	}

	#region Health Potion Item
	public void Heal(float amount)
	{
		player.PlayerCurHealth += amount;
		effectChecker.HealingEffect();
	}
	#endregion

	#region Battle Booster Item
	public void ApplyBattleBooster()
	{
		player.SetInvincible(true);
		effectChecker.BattleBoosterEffect(true);
	}

	public void RemoveBattleBooster()
	{
		player.SetInvincible(false);
		effectChecker.BattleBoosterEffect(false);
	}
	#endregion

	#region Shield Item
	public void GetShield()
	{
		player.SetShieldOn();
		effectChecker.ShieldEffect(true);
	}
	#endregion

	#region Coin Item
	public void GetCoin(int amount)
	{
		player.AddGold(amount);
		effectChecker.DropCoinEffect();
	}
	#endregion

	#region Coin Magnet Item
	public void ApplyCoinMagnet(float range)
	{
		magnetCtrl.ActivateMagnet(range);
		effectChecker.CoinMagnetEffect(true);
	}

	public void RemoveCoinMagnet()
	{
		magnetCtrl.DeactiveMagnet();
		effectChecker.CoinMagnetEffect(false);
	}
	#endregion

	#region Fatal Elixir Item
	public void ApplyFatalElixir(int inhance)
	{
		player.PlayerFatalRate += inhance;
		effectChecker.FatalElixirEffect(true);
	}

	public void RemoveFatalElixir(int inhance)
	{
		player.PlayerFatalRate -= inhance;
		effectChecker.FatalElixirEffect(false);
	}
	#endregion

	public void ResetAllItemApply()
	{
		RemoveFatalElixir(0);
		RemoveCoinMagnet();
		RemoveBattleBooster();

		effectChecker.ShieldEffect(false);
	}
}
