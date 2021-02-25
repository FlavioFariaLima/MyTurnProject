using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Dices
{   

    public static int[] RollDices(int dice, int bonus)
    {
        int result = Random.Range(1, dice);
        int finalResult = result + bonus;

        //                         dice result, dice + bonus
        int[] resultArray = new int[] { result, finalResult, dice, bonus };

        return resultArray;
    }
}

public static class MatchEvents
{
    public static List<MatchCharacter> RollIniciatives(List<MatchCharacter> characters)
    {
        List<MatchCharacter> sortedList = characters;

        foreach (MatchCharacter c in sortedList)
        {
            c.iniciative = Dices.RollDices(20, 0)[1];
        }

        sortedList = sortedList.OrderByDescending(w => w.iniciative).ToList();

        return sortedList;

    }
}
