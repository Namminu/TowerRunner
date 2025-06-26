using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum ItemUseTrigger
{
    PreItem = 0,
    OnDrop = 1
}

public abstract class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;
    public ItemUseTrigger useTrigger;
    public int maxStack;

    public abstract void Apply(Player player);
}

[CreateAssetMenu(menuName = "ItemData/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [Tooltip("All Listed Items")]
    public List<ItemData> allItems = new List<ItemData>();

    public ItemData GetItemById(int id)
    {
        return allItems.Find(item =>  item.id == id);
    }
}