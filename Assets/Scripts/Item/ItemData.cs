using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int itemPrice;

    [Tooltip("Item Prefab has PickUp Component for Spawn on Scene in Runtime")]
    public AssetReferenceGameObject pickupPrefab;

    public abstract void Apply(Player player);
}