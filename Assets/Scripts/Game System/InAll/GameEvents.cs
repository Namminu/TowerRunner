using System;

public static class GameEvents
{
    public static event Action<ItemData> OnShopItemSeledted;
	/// <summary>
	/// Called when selecting an item in the Shop Scroll
	/// </summary>
	public static void RaiseShopItemSeleted(ItemData item)
		=> OnShopItemSeledted?.Invoke(item);


	public static event Action<ItemData> OnItemBuyConfirmed;
	/// <summary>
	/// Called when an item purchase is confirmed in the Shop PopUp UI.
	/// </summary>
	public static void RaiseShopItemBuyConfirmed(ItemData item)
		=> OnItemBuyConfirmed?.Invoke(item);


	public static event Action OnInvenFull;
	/// <summary>
	/// Call when all 3 slots of the item inventory are full
	/// </summary>
	public static void RaiseInvenFull()
		=> OnInvenFull?.Invoke();


	public static event Action OnInventoryChanged;
	/// <summary>
	/// Called when a change occurs in the inventory
	/// </summary>
	public static void RaiseInventoryChanged()
		=> OnInventoryChanged?.Invoke();


	public static event Action OnShortageGold;
	/// <summary>
	/// Called when the player's gold isn't enough
	/// </summary>
	public static void RaiseShortageGold()
	=> OnShortageGold?.Invoke();


	public static event Action<int> OnGoldChanged;
	/// <summary>
	/// Called when the player's gold amount changed
	/// </summary>
	public static void RaiseGoldChanged(int num)
		=> OnGoldChanged?.Invoke(num);

	public static event Action<int> OnScoreChanged;
	/// <summary>
	/// Called when the current play score changes
	/// </summary>
	public static void RaiseScoreChanged(int num)
		=> OnScoreChanged?.Invoke(num);


	public static event Action<int> OnHighScoreChanged;
	/// <summary>
	/// Called when the highest score in the game is updated
	/// </summary>
	public static void RaiseHighScoreChanged(int num)
		=> OnHighScoreChanged?.Invoke(num);


	public static event Action OnBattleStarted;
	/// <summary>
	/// Called when battle starts
	/// </summary>
	public static void RaiseBattleStart()
		=> OnBattleStarted?.Invoke();

	/// <summary>
	/// Call at the end of the battle
	/// </summary>
	public static event Action OnBattleEnded;
	public static void RaiseBattleEnd()
		=> OnBattleEnded?.Invoke();

	/// <summary>
	/// Call When the player falls into a hole (PlaneHole)
	/// </summary>
	public static event Action OnPlayerFallInHole;
	public static void RaiseFallInHole()
		=> OnPlayerFallInHole?.Invoke();
}
