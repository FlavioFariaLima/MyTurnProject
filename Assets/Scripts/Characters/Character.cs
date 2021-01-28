using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "UltraMare/Characters/Character", order = 3)]
public class Character : ScriptableObject
{
    public string characterName;

    // Stats
    [Header("Character Experience")]
    public int level;
    public int xp;

    public Race race;
    public Classe classe;

    [Header("strength - constitution - dexterity - intelligence - wisdow - charisma")]
    public int[] abilitiesValues;
    private Abilities abilities;

    // Combat
    public int health;
    public int armorClass;

    // BaseAttack
    private int baseAttack;
    public int BaseAttack
    {
        get { return baseAttack; }
        set { baseAttack = level / value; }
    }

    public int meleeAttack;
    public int meleeDamage;
    public float meleeDistance;

    public int rangeAttack;
    public int rangeDamage;
    public float rangeDistance;

    // Resistences
    public Resistences resistences;

    // Movement
    public int movement;

    // Get/Set Variables
    public Abilities GetAbilities
    {
        get { return abilities; }
        set { abilities = value; }
    }

    public void UpdateCharacterInfo(string name, int level, Race race, Classe classe, Abilities abilities)
    {
        this.characterName = name;
        this.level = level;
        this.race = race;
        this.classe = classe;
        this.abilities = AbilitiesAutoComplete(abilitiesValues[0], abilitiesValues[1], abilitiesValues[2], abilitiesValues[3], abilitiesValues[4], abilitiesValues[5]);

        this.health = (classe.healthDice + this.abilities.constitution[1]) * level;
        this.armorClass = 10;
        this.baseAttack = classe.attackBonus[level - 1];

        this.meleeAttack = this.abilities.strength[1] + classe.attackBonus[level - 1];
        this.rangeAttack = this.abilities.dexterity[1] + classe.attackBonus[level - 1];

        this.movement = race.movement;

        this.resistences = new Resistences(this);
    }

    /// <summary>
    ///  Temp Stuff
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    public Abilities AbilitiesAutoComplete(int str, int con, int dex, int inte, int wis, int cha)
    {
        /*
        strength;
        constitution
        dexterity;
        intelligence
        wisdow;
        charisma;
        */

        Abilities newAbilities = new Abilities();
        Dictionary<int, int> bonusValues = new Dictionary<int, int>();

        bonusValues.Add(1, -5);
        bonusValues.Add(2, -4);
        bonusValues.Add(3, -5);
        bonusValues.Add(4, -3);
        bonusValues.Add(5, -3);
        bonusValues.Add(6, -2);
        bonusValues.Add(7, -2);
        bonusValues.Add(8, -1);
        bonusValues.Add(9, -1);
        bonusValues.Add(10, 0);
        bonusValues.Add(11, 0);
        bonusValues.Add(12, 1);
        bonusValues.Add(13, 1);
        bonusValues.Add(14, 2);
        bonusValues.Add(15, 2);
        bonusValues.Add(16, 3);
        bonusValues.Add(17, 3);
        bonusValues.Add(18, 4);
        bonusValues.Add(19, 4);
        bonusValues.Add(20, 5);
        bonusValues.Add(21, 5);
        bonusValues.Add(22, 6);
        bonusValues.Add(23, 6);
        bonusValues.Add(24, 7);
        bonusValues.Add(25, 7);
        bonusValues.Add(26, 8);
        bonusValues.Add(27, 8);
        bonusValues.Add(28, 9);
        bonusValues.Add(29, 9);
        bonusValues.Add(30, 10);
        bonusValues.Add(31, 10);
        bonusValues.Add(32, 11);
        bonusValues.Add(33, 11);
        ////////////////////////

        newAbilities.strength = new int[2] { str, bonusValues[str] };
        newAbilities.constitution = new int[2] { con, bonusValues[con] };
        newAbilities.dexterity = new int[2] { dex, bonusValues[dex] };
        newAbilities.intelligence = new int[2] { inte, bonusValues[inte] };
        newAbilities.wisdow = new int[2] { wis, bonusValues[wis] };
        newAbilities.charisma = new int[2] { cha, bonusValues[cha] };

        return newAbilities;
    }    
}


[Serializable]
public class Abilities
{
    public int[] strength;
    public int[] constitution;
    public int[] dexterity;
    public int[] intelligence;
    public int[] wisdow;
    public int[] charisma;

}

[Serializable]
public class Resistences
{
    public int[] fortitude = new int[5];
    public int[] reflex = new int[5];
    public int[] will = new int[5];

    public Resistences(Character character)
    {
        this.fortitude[1] = character.GetAbilities.constitution[1];
        this.fortitude[2] = character.classe.fortitudeBonus[character.level];
        this.fortitude[3] = 0;
        this.fortitude[4] = 0;

        this.fortitude[0] = this.fortitude[1] + this.fortitude[2] + this.fortitude[3] + this.fortitude[4];
    }
}

[Serializable]
public class Skills
{
    public int balance;
    public int concentration;
    public int decipherScript;
    public int disableDevice;
    public int handleAnimal;
    public int heal;
    public int hide;
    public int intimidate;
    public int jump;
    public int knowledge;
    public int moveSilently;
    public int openLock;
    public int perception;
    public int search;

}


