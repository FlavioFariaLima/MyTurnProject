using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "UltraMare/Create Item", order = 1)]
public class ItemBlueprint : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public Sprite itemIcon;

    [SerializeField] public string description;
    public MyParameters.ItemType itemType;
    public List<MyParameters.ItemProperties> itemProperties;

    [SerializeField] public float weight;
    [SerializeField] public bool canStock;
    [SerializeField] public int maxStock;

    [SerializeField] public float onUseValue;
    [SerializeField] public float condition;

    // Alternative Status
    [SerializeField] public List<string> alternativeNames = new List<string>();
    [SerializeField] public List<Sprite> alternativeIcons = new List<Sprite>();
}
