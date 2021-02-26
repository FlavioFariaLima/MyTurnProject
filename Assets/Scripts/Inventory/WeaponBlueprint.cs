using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "UltraMare/Create Weapon", order = 2)]
public class WeaponBlueprint : ItemBlueprint
{
    [Header("Weapon Stats")]
    [SerializeField] public float cost;
    [SerializeField] public int dmgS;
    [SerializeField] public int dmgM;
    [SerializeField] public int critical;
    [SerializeField] public int rangeIncrement;
    [SerializeField] public WeaponType[] weaponType;
}
