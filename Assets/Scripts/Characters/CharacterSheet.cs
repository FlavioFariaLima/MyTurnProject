using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterSheet: MonoBehaviour
{
    // Components
    [SerializeField] private int charId;
    [SerializeField] private string charName;
    [SerializeField] private int playerId;

    [HideInInspector] public PlayerCharacterController controller; // Set by CharacterController      
    [HideInInspector] public bool isMyTurn;

    // Craft
    [SerializeField] private List<ItemRecipe> knowRecipes;
    [SerializeField] private Character characterInfo;

    // Equipment
    [SerializeField] private int totalAttacks = 1;
    public int TotalAttacks
    {
        get { return totalAttacks; }
        set { totalAttacks = value; }
    }
    [SerializeField] private int numberOfAttacks = 0;
    public int NumberOfAttack
    {
        get { return numberOfAttacks; }
        set { numberOfAttacks = value; }
    }

    [SerializeField] private WeaponStats meleeWeapon;
    [SerializeField] private WeaponStats rangeWeapon;

    // Health
    private int currentHealth = 0;
    private bool isAlive = true;

    // Match Variables
    private int matchIniciative = -1;
    public int MatchIniciative
    {
        get { return matchIniciative; }
        set { matchIniciative = value; }
    }

    private GameObject charIcon;
    public GameObject CharIcon
    {
        get { return charIcon; }
        set { charIcon = value; }
    }

    public GameObject shotcutIcon;

    //
    public void RestartStats()
    {
        numberOfAttacks = 0;
    }

    private void Awake()
    {
        characterInfo.UpdateCharacterInfo(characterInfo.name, characterInfo.level, characterInfo.race, characterInfo.classe, characterInfo.GetAbilities);
        playerId = GetComponentInParent<Player>().GetId();

        currentHealth = characterInfo.health;
        RestartStats();
    }

    private void Update()
    {
    }

    /// <summary>
    /// Get Stats =>
    /// </summary>
    /// <returns></returns>

    public Sprite Portrait()
    {
        return characterInfo.portrait;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public int GetId()
    {
        return charId;
    }

    public void SetId(bool reset)
    {
        if (reset)
            charId = 0;
        else
            charId = GetInstanceID();
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public string GetName()
    {
        return charName;
    }

    public void SetName(string newName)
    {
        charName = newName;
        gameObject.name = charName;
    }

    public Classe GetClass()
    {
        return characterInfo.classe;
    }

    public Race GetRace()
    {
        return characterInfo.race;
    }

    public int GetHealth()
    {
        return characterInfo.health;
    }

    public int GetCurrrentHelth()
    {
        return currentHealth;
    }

    public int GetArmour()
    {
        return characterInfo.armorClass;
    }

    /// <summary>
    /// Attack and Damage
    /// </summary>
    /// <returns></returns>
    public int GetMeleeAttack()
    {
        return characterInfo.meleeAttack;
    }

    public int GetMeleeDamage()
    {
        if (controller.Equipment().GetEquippedMeleeWeapon() != null)
            characterInfo.meleeDamage = controller.Equipment().GetEquippedMeleeWeapon().weaponStats.dmgM;
        else
            characterInfo.meleeDamage = 2;

        return characterInfo.meleeDamage;
    }

    public int GetRangeAttack()
    {
        return characterInfo.rangeAttack;
    }

    public int GetRangeDamage()
    {
        if (controller.Equipment().GetEquippedRangeWeapon() != null)
            characterInfo.rangeDamage = controller.Equipment().GetEquippedRangeWeapon().weaponStats.dmgM;
        else
            characterInfo.rangeDamage = 2;

        return characterInfo.rangeDamage;
    }
    
    public int GetMovement()
    {
        return characterInfo.movement;
    }

    public float GetMeleeDistance()
    {
        return characterInfo.meleeDistance;
    }

    public float GetRangeDistance()
    {
        return characterInfo.rangeDistance;
    }

    public Abilities GetAbilities()
    {
        return characterInfo.GetAbilities;
    }

    public List<ItemRecipe> GetKnowRecipes()
    {
        return knowRecipes;
    }

    // Set Stats
    public void AddKnowRecipe(ItemRecipe recipe)
    {
        knowRecipes.Add(recipe);
    }

    ////////////////////////
    ///

    // Passive Actions
    public void TakeDamage(int dmg, CharacterSheet whoHitMe)
    {
        currentHealth -= dmg;

        if (currentHealth < 0 && isAlive)
        {
            Dead();
        }

        Global.UI.UpdateCurrentCharacterInfo();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
    }

    public void Dead()
    {
        // Character is Dead
        isAlive = false;
        controller.SetAnimatorDead();

        Debug.Log($"{GetName()} is dead!");

        // Deal with Match Icons
        CharIcon.transform.GetComponent<Image>().color = new Color(1, 0, 0, 1);

        if(shotcutIcon)
        {
            shotcutIcon.transform.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }

        // Check if Match is Over
        Global.Match.CheckForMatchEnd();
    }
}

[Serializable]
public class Actions
{
    public enum AttackType
    {
        melee = 0,
        range = 1,
        magic = 2,
    }
}

[Serializable]
public class Creature
{
    public enum Size
    {
        small = -1,
        medium = 0,
        big = 1
    }
}
