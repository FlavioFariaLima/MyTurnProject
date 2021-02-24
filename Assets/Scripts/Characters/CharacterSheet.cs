using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSheet: MonoBehaviour
{
    // Components
    [SerializeField] private int charId;
    [SerializeField] private int playerId;
    [SerializeField] private Sprite charPortrait;
    public  Sprite CharPortrait
    {
        get { return charPortrait; }
    }

    [HideInInspector] public PlayerCharacterController controller; // Set by CharacterController      
    [HideInInspector] public bool isMyTurn;

    // Craft
    [SerializeField] private List<ItemRecipe> knowRecipes;
    [SerializeField] private Character characterInfo;

    // Equipment
    [SerializeField] private Weapon meleeWeapon;
    [SerializeField] private Weapon rangeWeapon;

    // Health
    private int currentHealth = 0;
    private bool isAlive = true;

    private GameObject charIcon;
    public GameObject CharIcon
    {
        get { return charIcon; }
        set { charIcon = value; }
    }

    private void Awake()
    {
        characterInfo.UpdateCharacterInfo(characterInfo.name, characterInfo.level, characterInfo.race, characterInfo.classe, characterInfo.GetAbilities);
        playerId = GetComponentInParent<Player>().GetId();

        currentHealth = characterInfo.health;
    }

    private void Update()
    {
    }

    /// <summary>
    /// Get Stats =>
    /// </summary>
    /// <returns></returns>

    public bool IsAlive()
    {
        return isAlive;
    }

    public int GetId()
    {
        return charId;
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public string GetName()
    {
        return characterInfo.characterName;
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
        return characterInfo.meleeDamage;
    }

    public int GetRangeAttack()
    {
        return characterInfo.rangeAttack;
    }

    public int GetRangeDamage()
    {
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
        Global.Match.UpdateIconHealthBar(this);

        if (currentHealth < 0 && isAlive)
        {
            Dead();
        }
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

        CharIcon.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
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

[Serializable]
public class Weapon
{
    private string weaponName;
    private int damage;

    public string WeaponName()
    {
        return weaponName;
    }
    public int WeaponDamage()
    {
        return damage;
    }
}
