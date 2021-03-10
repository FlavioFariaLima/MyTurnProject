using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private int playerId;
    [SerializeField] private List<CharacterSheet> playerCharacters;
    [SerializeField] private bool playerIsAI;
    [SerializeField] private bool isMainPlayer;
    [SerializeField] public int charactersCount;


    // Public Variables Methods
    public bool IsMainPlayer()
    {
        return isMainPlayer;
    }

    public int CharactersCount
    {
        get { return charactersCount; }
        set { charactersCount = value; }
    }

    public bool PlayerIsAI
    {
        get { return playerIsAI; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (playerIsAI)
        {
            foreach(Transform child in transform)
            {
                child.GetComponent<AICharacterController>().StartAI(child.GetComponent<PlayerCharacterController>(), child.GetComponent<CharacterSheet>());
            }
        }
    }

    /// <summary>
    /// Get Player Information
    /// </summary>
    public string GetName
    {
        get { return playerName; }

        set { playerName = value; }
    }

    public int GetId()
    {
        return playerId;
    }

    public List<CharacterSheet> PlayerCharacters
    {
        get { return playerCharacters; }

        set { playerCharacters = value; }
    }    
}
