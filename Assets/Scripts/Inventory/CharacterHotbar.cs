using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterHotbar : MonoBehaviour
{

    [SerializeField] public Dictionary<int, UltraMare.Item> Items = new Dictionary<int, UltraMare.Item>();
    [SerializeField] private int maxSpace = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool AddItem(UltraMare.Item item, int index, int myIndex, bool isChangingPos)
    {
        if (Items.Count >= maxSpace)
        {
            Debug.Log("Not enough space.");
            return false;
        }

        if (!Items.ContainsKey(index))
        {
            Items.Add(index, item);

            if (Items.ContainsKey(myIndex) && isChangingPos)
            {
                RemoveItem(myIndex);
            }
        }
        else
        {
            UltraMare.Item oldItem = Items[index];
            Items[myIndex] = oldItem;
            Items[index] = item;
        }

        Global.UI.UpdateCharacterInventory();

        return true;
    }

    public void RemoveItem(int index)
    {
        Items.Remove(index);

        Global.UI.UpdateCharacterInventory();
    }
}
