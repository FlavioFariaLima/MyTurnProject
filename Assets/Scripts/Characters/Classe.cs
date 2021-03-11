using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Classe", menuName = "UltraMare/Characters/Classe", order = 2)]
[Serializable]
public class Classe : ScriptableObject
{
    public string classeName;

    // Class Bonus
    public int healthDice;
    public int[] attackBonus;
    public int[] fortitudeBonus;
    public int[] reflexBonus;
    public int[] willBonus;
}
