using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "UltraMare/Create Item", order = 1)]
public class ItemBlueprint : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public ItemRarity rarity;
    public string description;

    [Header("Item Basic Stats")]
    public List<MyParameters.ItemProperties> itemProperties;
    public float condition;
    public float weight;
    public bool canStock;
    public int maxStock;
    public float onUseValue;

    [Header("Item Alternative Information")]
    public List<string> alternativeNames = new List<string>();
    public List<Sprite> alternativeIcons = new List<Sprite>();

    [Header("Weapon Stats")]
    public float cost;
    public int dmgS;
    public int dmgM;
    public int critical;
    public int criticalMultiply;
    public int rangeIncrement;
    public bool rangeOnly;
    public WeaponType[] weaponType;
}
