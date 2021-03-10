using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PseudoCharacter : MonoBehaviour
{    
    [SerializeField] private CharacterSheet myCharacter;

    public CharacterSheet MyCharacter()
    {
        return myCharacter;
    }

    public void MyCharacter(CharacterSheet character)
    {
        myCharacter = character;
        GetComponentInChildren<TextMeshProUGUI>().text = myCharacter.GetName();
    }
}
