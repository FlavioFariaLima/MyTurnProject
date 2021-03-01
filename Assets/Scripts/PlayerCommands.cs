using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommands : MonoBehaviour
{
    List<PlayerCharacterController> selectedCharacters;
    public bool playerIsAttacking = false;
    public Actions.AttackType attackType;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacters = new List<PlayerCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearSelectedCharacters()
    {
        foreach (PlayerCharacterController character in selectedCharacters)
        {
            character.SelectedForAct(false);
        }

        selectedCharacters.Clear();
    }

    public void AddSelectedCharacter(PlayerCharacterController charControl)
    {
        if (!selectedCharacters.Contains(charControl))
            selectedCharacters.Add(charControl);
    }

    public List<PlayerCharacterController> GetSelectedCharacters()
    {
        return selectedCharacters;
    }

    public Transform GetMainSelectedCharacterTransform()
    {
        return selectedCharacters[0].transform;
    }

    public Transform GetLastSelectedCharacter()
    {
        return selectedCharacters[selectedCharacters.Count - 1].transform;
    }
}
