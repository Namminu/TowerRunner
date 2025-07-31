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








}
