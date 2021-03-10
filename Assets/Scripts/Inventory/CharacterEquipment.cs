using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    public bool hasMeleeWeapon = false;
    public bool hasRangeWeapon = false;
    [HideInInspector] public int meleeWeaponIndex = -1;

    [HideInInspector] public PlayerCharacterController controller;
    [HideInInspector] public Dictionary<int, Item> equipments = new Dictionary<int, Item>();

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<PlayerCharacterController>();
    }

    public Item GetEquippedMeleeWeapon()
    {
        if(CheckIfHasWeapon()[0])
            return equipments[meleeWeaponIndex];
        else
            return null;
    }

    public Item GetEquippedRangeWeapon()
    {
        if (CheckIfHasWeapon()[1])
            return equipments[2];
        else
            return null;
    }

    public bool[] CheckIfHasWeapon()
    {
        bool[] r = new bool[2];
        r[0] = false;
        r[1] = false;

        if (equipments.ContainsKey(2))
        {
            hasRangeWeapon = true;
            r[1] = true;
        }

        if (equipments.ContainsKey(0))
        {
            meleeWeaponIndex = 0;
            hasMeleeWeapon = true;
            r[0] = true;
        }
        else if(equipments.ContainsKey(1))
        {
            meleeWeaponIndex = 1;
            hasMeleeWeapon = true;
            r[0] = true;
        }

        if (controller.character.isMyTurn)
        {
            // Active Icons
            if (r[1])
                Global.UI.EnableActionButton("Range", true, controller);
            else
                Global.UI.EnableActionButton("Range", false, controller);
        }

        return r;
    }

    public bool AddEquipment(Item item, ItemType type, int slotIndex)
    {
        bool r = false;

        if (type == ItemType.weapon && !item.weaponStats.rangeOnly)
        {
            if (slotIndex == 0 || slotIndex == 1)
            {

                if (equipments.ContainsKey(slotIndex))
                {
                    Global.UI.characterSelectedToUI.controller.Inventory().AddItem(equipments[slotIndex]);
                    RemoveEquipment(slotIndex);
                }

                equipments.Add(slotIndex, item);
                r = true;
            }
        }
        else if (type == ItemType.weapon && item.weaponStats.rangeOnly)
        {
            if (slotIndex == 2)
            {
                if (equipments.ContainsKey(slotIndex))
                {
                    Global.UI.characterSelectedToUI.controller.Inventory().AddItem(item);
                    RemoveEquipment(slotIndex);
                }

                equipments.Add(slotIndex, item);
                r = true;
            }
        }

        CheckIfHasWeapon();
        Global.UI.UpdateCharacterEquipment();
        return r;
    }

    public void RemoveEquipment(int slotIndex)
    {
        equipments.Remove(slotIndex);

        CheckIfHasWeapon();
        Global.UI.UpdateCharacterEquipment();
    }
}
