using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "UltraMare/Create Recipe", order = 2)]
public class ItemRecipe : ScriptableObject
{
    [SerializeField] public ItemBlueprint ResultItem;
    [SerializeField] public List<RecipeIngredients> RecipeIngredients;

    [SerializeField] public int TimeToMake;
}

[Serializable]
public struct RecipeIngredients
{
    public int amount;
    public ItemBlueprint item;
}
