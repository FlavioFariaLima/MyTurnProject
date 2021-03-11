using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "UltraMare/Characters/Race", order = 1)]
[Serializable]
public class Race : ScriptableObject
{
    public string raceName;

    // Race Bonus
    public Creature.Size size;
    public int movement;
    public int darkVision;
    public Abilities abilitiesBonus;
    public Skills skill;
}
