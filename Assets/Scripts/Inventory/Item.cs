using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;
    public Sprite itemIcon;

    public string description;
    public MyParameters.ItemType itemType;
    public List<MyParameters.ItemProperties> itemProperties;

    public float weight;
    public bool canStock;
    public int maxStock;
    public int quantity;

    public float onUseValue;
    public float condition;

    public ItemBlueprint itemBlueprint;

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

        return newItem;
    }

    public int ItemQuatity()
    {
        return quantity;
    }
}
