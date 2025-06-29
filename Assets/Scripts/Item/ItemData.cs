using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;

    public abstract void Apply(Player player);
}