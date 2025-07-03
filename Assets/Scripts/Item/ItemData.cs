using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;

    [Tooltip("Item Prefab has PickUp Component for Spawn on Scene in Runtime")]
    public ItemPickup pickupPrefab;

    public abstract void Apply(Player player);
}