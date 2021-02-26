using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory: MonoBehaviour
{
    [SerializeField] private string inventoryName;

    [SerializeField] private float maxWeight;
    [SerializeField] private float curWeight;
    [HideInInspector] private int size = 48;

    [SerializeField] List<UltraMare.Item> inventoryItems = new List<UltraMare.Item>();

    // Events
    public event Action OnAdd = () => { };
    public event Action OnRemove = () => { };

    public int InventorySize()
    {
        return size;
    }

    public List<UltraMare.Item> Items
    {
        get
        {
            return inventoryItems;
        }
        set
        {
            inventoryItems = value;
        }
    }

    public bool AddItem(UltraMare.Item _item)
    {
        if (Items.Count < size)
        {
            try
            {
                inventoryItems.Add(_item);

                Global.UI.UpdateCharacterInventory();

                if (_item.itemProperties.Contains(MyParameters.ItemProperties.fuel))
                {
                    try
                    {
                        GetComponent<CraftStation>().AddFuel(_item.onUseValue);
                    }
                    catch
                    {
                        //Debug.Log($"Fail to compute {_item} fuel!");
                    }
                }

                OnAdd();
                return true;
            }
            catch
            {
                Debug.Log($"Cant add item {_item}");
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool RemoveItem(int index)
    {
        try
        {
            if (Items[index].itemProperties.Contains(MyParameters.ItemProperties.fuel))
            {
                try
                {
                    GetComponent<CraftStation>().RemoveFuel(Items[index].onUseValue);
                }
                catch
                {

                }
            }

            inventoryItems.RemoveAt(index);
            Global.UI.UpdateCharacterInventory();

            OnRemove();
            return true;
        }
        catch
        {
            Debug.Log($"Cant remove item {inventoryItems[index]}");
            return false;
        }
    }

    public void ChangePosition(int index, int slotID, UltraMare.Item item)
    {
        if (index < inventoryItems.Count)
        {
            inventoryItems.RemoveAt(slotID);
            inventoryItems.Insert(index, item);
        }
        else
        {
            inventoryItems.RemoveAt(slotID);
            inventoryItems.Insert(inventoryItems.Count, item);
        }


        Global.UI.UpdateCharacterInventory();
        Debug.Log("Change Item Position");
    }

}

