using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public ItemRarity rarity;
    public string description;

    public List<MyParameters.ItemProperties> itemProperties;

    public float weight;
    public bool canStock;
    public int maxStock;
    public int quantity;

    public float onUseValue;
    public float condition;

    public ItemBlueprint itemBlueprint;
    public WeaponStats weaponStats;

    // Alternative Status
    public List<string> alternativeNames;
    public List<Sprite> alternativeIcons;

    public static Item CreateNewItem(ItemBlueprint item)
    {
        Item newItem = new Item();

        newItem.itemName = item.itemName;
        newItem.itemIcon = item.itemIcon;

        newItem.itemProperties = new List<MyParameters.ItemProperties>();
        foreach (MyParameters.ItemProperties p in item.itemProperties)
        {
            newItem.itemProperties.Add(p);
        }

        newItem.itemType = item.itemType;
        newItem.rarity = item.rarity;
        newItem.description = item.description;

        newItem.weight = item.weight;
        newItem.canStock = item.canStock;
        newItem.maxStock = item.maxStock;

        newItem.quantity = 1;

        newItem.onUseValue = item.onUseValue;
        newItem.condition = item.condition;

        // Alternatives
        newItem.alternativeNames = new List<string>();
        foreach (string name in item.alternativeNames)
        {
            newItem.alternativeNames.Add(name);
        }

        newItem.alternativeIcons = new List<Sprite>();
        foreach (Sprite icon in item.alternativeIcons)
        {
            newItem.alternativeIcons.Add(icon);
        }

        // Weapon
        if (item.itemType == ItemType.weapon)
            newItem.weaponStats = new WeaponStats(item);


        return newItem;
    }

    public int ItemQuatity()
    {
        return quantity;
    }
}

public struct WeaponStats
{
    public float cost;
    public int dmgS;
    public int dmgM;
    public int critical;
    public int criticalMultiply;
    public int rangeIncrement;
    public bool rangeOnly;
    public WeaponType[] weaponType;

    public WeaponStats(ItemBlueprint blueprint)
    {
        cost = blueprint.cost;
        dmgS = blueprint.dmgS;
        dmgM = blueprint.dmgM;
        critical = blueprint.critical;
        criticalMultiply = blueprint.criticalMultiply;
        rangeIncrement = blueprint.rangeIncrement;
        weaponType = blueprint.weaponType;
        rangeOnly = blueprint.rangeOnly;
    }
}
